using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrossPlatform.Code.Enums;
using CrossPlatform.Code.Interfaces;
using CrossPlatform.Code.Json.TL;
using CrossPlatform.Code.Models;
using CrossPlatform.Code.Utils;
using AuthorJson = CrossPlatform.Code.Json.TL.AuthorJson;

namespace CrossPlatform.Code.Implementations;

public class PackProcessor : IPackProcessor
{
    private static readonly ISet<string> PreviewExtensions = new HashSet<string> {".jpg", ".png", ".gif", ".webp"};
    private const int PackProcessingTries = 20;
    private const int PackProcessingSleepMs = 500;

    public event Action<string>? PackLoadingStarted;

    public event Action<(string filePath, PackModel? loadedPack, Exception? error)>? PackLoaded;

    public event Action<(PackModel pack, string targetFilePath)>? PackSavingStarted;

    public event Action<(PackModel pack, string targetFilePath, Exception? error)>? PackSaved;

    private readonly IProgressManager? _loadProgressManager;
    private readonly IProgressManager? _saveProgressManager;
    private readonly IFileConverter _fileConverter;
    private readonly ISessionHelper _sessionHelper;
    private readonly IImageConverter _imageConverter;

    private readonly object _loadingLock = new();
    private readonly object _savingLock = new();

    public PackProcessor(
        IProgressManager? loadProgressManager,
        IProgressManager? saveProgressManager,
        IFileConverter fileConverter,
        ISessionHelper sessionHelper,
        IImageConverter imageConverter
    )
    {
        _loadProgressManager = loadProgressManager;
        _saveProgressManager = saveProgressManager;
        _fileConverter = fileConverter;
        _sessionHelper = sessionHelper;
        _imageConverter = imageConverter;

        if (loadProgressManager != null)
        {
            PackLoadingStarted += OnPackLoadingStarted;
            PackLoaded += OnPackLoaded;
        }

        if (saveProgressManager != null)
        {
            PackSavingStarted += OnPackSavingStarted;
            PackSaved += OnPackSaved;
        }
    }

    public void LoadPackFromFile(string filePath)
    {
        if (filePath == null)
            throw new ArgumentNullException(nameof(filePath));

        Task.Run(async () =>
        {
            try
            {
                PackLoadingStarted?.Invoke(filePath);
                var packModel = await LoadPackModelInternal(filePath);
                PackLoaded?.Invoke((filePath, packModel, null));
            }
            catch (Exception ex)
            {
                PackLoaded?.Invoke((filePath, null, ex));
            }
        });
    }

    public void SavePackToFile(PackModel pack, string targetFilePath)
    {
        if (pack == null)
            throw new ArgumentNullException(nameof(pack));
        if (targetFilePath == null)
            throw new ArgumentNullException(nameof(targetFilePath));

        Task.Run(() =>
        {
            try
            {
                PackSavingStarted?.Invoke((pack, targetFilePath));
                SavePackModelInternal(pack, targetFilePath);
                PackSaved?.Invoke((pack, targetFilePath, null));
            }
            catch (Exception ex)
            {
                PackSaved?.Invoke((pack, targetFilePath, ex));
            }
        });
    }

    private void OnPackLoadingStarted(string item)
    {
        lock (_loadingLock)
        {
            Debug.Assert(_loadProgressManager != null, nameof(_loadProgressManager) + " != null");
            _loadProgressManager.RemainingFilesCount++;
        }
    }

    private void OnPackLoaded((string filePath, PackModel? loadedPack, Exception? error) item)
    {
        lock (_loadingLock)
        {
            Debug.Assert(_loadProgressManager != null, nameof(_loadProgressManager) + " != null");
            _loadProgressManager.RemainingFilesCount--;
        }
    }

    private void OnPackSavingStarted((PackModel pack, string targetFilePath) item)
    {
        lock (_savingLock)
        {
            Debug.Assert(_saveProgressManager != null, nameof(_saveProgressManager) + " != null");
            _saveProgressManager.RemainingFilesCount++;
        }
    }

    private void OnPackSaved((PackModel pack, string targetFilePath, Exception? error) item)
    {
        lock (_savingLock)
        {
            Debug.Assert(_saveProgressManager != null, nameof(_saveProgressManager) + " != null");
            _saveProgressManager.RemainingFilesCount--;
        }
    }

    private async Task<PackModel> LoadPackModelInternal(string filePath)
    {
        string targetFolderPath = _sessionHelper.GenerateNonExistentDirPath();
        using (var zip = ZipFile.OpenRead(filePath))
            zip.ExtractToDirectory(targetFolderPath);

        string packSettingsFile = Path.Combine(targetFolderPath, "Settings.json");
        string packIconGif = Path.Combine(targetFolderPath, "Icon.gif");
        string packIconPng = Path.Combine(targetFolderPath, "Icon.png");
        string packIconWebP = Path.Combine(targetFolderPath, "Icon.webp");
        string packPreviewsFolder = Path.Combine(targetFolderPath, "Previews");
        string packAuthorsFolder = Path.Combine(targetFolderPath, "Authors");
        string packModifiedFilesFolder = Path.Combine(targetFolderPath, "Modified");

        string packSettingsText = await File.ReadAllTextAsync(packSettingsFile, Encoding.UTF8);
        PackSettings packSettings = PackSettings.Processor.Deserialize(packSettingsText);

        string? packIconFile = null;
        if (File.Exists(packIconGif))
            packIconFile = packIconGif;
        else if (File.Exists(packIconPng))
            packIconFile = packIconPng;
        else if (File.Exists(packIconWebP))
            packIconFile = _imageConverter.ConvertWebPToTempPngFile(packIconWebP);

        string[] previewsPaths = 
            Directory.Exists(packPreviewsFolder)
                ? Directory.EnumerateFiles(packPreviewsFolder)
                    .Where(it => PreviewExtensions.Contains(Path.GetExtension(it)))
                    .ToArray()
                : Array.Empty<string>();

        string[] modifiedFileExts = PackUtils.PacksInfo.Select(it => it.ConvertedFilesExt).ToArray();
        string[] modifiedFilesPaths =
            Directory.Exists(packModifiedFilesFolder)
                ? Directory.EnumerateFileSystemEntries(packModifiedFilesFolder)
                    .Where(it => modifiedFileExts.Contains(Path.GetExtension(it)))
                    .ToArray()
                : Array.Empty<string>();

        var authors = packSettings.Authors?
            .ConvertAll(it => JsonToAuthorModel(it, packAuthorsFolder))
            .ToArray();

        var modifiedFiles = new List<PackModel.ModifiedFile>();
        foreach (string modifiedFile in modifiedFilesPaths)
        {
            string? configFile = Path.ChangeExtension(modifiedFile, PackUtils.PackFileConfigExtension);
            if (!File.Exists(configFile)) {
                configFile = null;
            }

            string fileExt = Path.GetExtension(modifiedFile);
            FileType fileType = PackUtils.PacksInfo.First(it => it.ConvertedFilesExt == fileExt).FileType;
            var (sourceFile, fileInfo) = await _fileConverter.ConvertToSource(
                packStructureVersion: packSettings.PackStructureVersion,
                fileType: fileType,
                targetFile: modifiedFile,
                configFile: configFile
            );
            modifiedFiles.Add(
                new PackModel.ModifiedFile(
                    Config: fileInfo,
                    FilePath: sourceFile,
                    FileType: fileType
                )
            );
        }
    
        return new PackModel(
            Authors: authors ?? Array.Empty<PackModel.Author>(),
            PreviewsPaths: previewsPaths,
            ModifiedFiles: modifiedFiles.ToArray(),
            PackStructureVersion: packSettings.PackStructureVersion,
            IconFilePath: packIconFile ?? string.Empty,
            Title: packSettings.Title,
            DescriptionRussian: packSettings.DescriptionRussian ?? string.Empty,
            DescriptionEnglish: packSettings.DescriptionEnglish ?? string.Empty,
            Guid: packSettings.Guid,
            Version: packSettings.Version,
            IsBonusPack: packSettings.IsBonus,
            PredefinedTags: packSettings.PredefinedTags?.ToList() ?? new List<PredefinedPackTag>(0)
        );
    }

    private void SavePackModelInternal(PackModel packModel, string filePath)
    {
        var authorsMappings = new List<(ImageInfo? sourceFile, string? targetFile, AuthorJson json)>();
        int authorFileIndex = 1;
        foreach (var author in packModel.Authors) {
            string? fileExtension;
            AuthorJson json = AuthorModelToJson(author, ref authorFileIndex, out bool copyIcon, out fileExtension);
            authorsMappings.Add((copyIcon ? author.Icon : null, copyIcon ? $"{authorFileIndex - 1}{fileExtension}" : null, json));
        }

        (1 / (1 / BonusTypeEnum.Length)).Ignore();
        var packSettingsJson = new PackSettings(
            packStructureVersion: packModel.PackStructureVersion,
            title: packModel.Title,
            descriptionEnglish: packModel.DescriptionEnglish,
            descriptionRussian: packModel.DescriptionRussian,
            version: packModel.Version,
            guid: packModel.Guid,
            authors: authorsMappings.ConvertAll(it => it.json),
            predefinedTags: packModel.PredefinedTags.ToList(),
            isBonus: packModel.IsBonusPack,
            // todo: change when new types are available
            bonusType: BonusType.OldVersionOwners
        );

        IOUtils.TryDeleteFile(filePath, PackProcessingTries, PackProcessingSleepMs);

        var compressionLevel = CompressionLevel.SmallestSize;
        using (var zip = ZipFile.Open(filePath, ZipArchiveMode.Create))
        {
            zip.CreateEntry(".nomedia");

            if (!string.IsNullOrEmpty(packModel.IconFilePath))
            {
                string targetPath = Path.GetExtension(packModel.IconFilePath) == ".gif"
                    ? packModel.IconFilePath
                    : IOUtils.ChooseLighterFileAndDeleteSecond(
                        packModel.IconFilePath,
                        _imageConverter.ConvertImageToTempWebPFile(packModel.IconFilePath, lossless: true)
                    );

                zip.CreateEntryFromFile(
                    sourceFileName: targetPath,
                    entryName: $"Icon{Path.GetExtension(targetPath)}",
                    compressionLevel
                );
            }

            if (authorsMappings.Any())
            {
                zip.CreateEntry("Authors/.nomedia");
                foreach (var (sourceFile, targetFile, json) in authorsMappings)
                    if (sourceFile != null)
                    {
                        if (sourceFile.Type == ImageInfo.ImageType.Gif)
                        {
                            CreateEntryFromBytes(zip, sourceFile.Bytes, entryName: $"Authors/{targetFile}", compressionLevel: compressionLevel);
                        }
                        else
                        {
                            string webPImage = _imageConverter.ConvertImageToTempWebPFile(sourceFile.Bytes, lossless: true);
                            if (new FileInfo(webPImage).Length < sourceFile.Bytes.Length)
                            {
                                json.File = Path.ChangeExtension(targetFile, ".webp");
                                string compressedFileName = $"Authors/{json.File}";
                                zip.CreateEntryFromFile(
                                    sourceFileName: webPImage,
                                    entryName: compressedFileName,
                                    compressionLevel
                                );
                            }
                            else
                            {
                                File.Delete(webPImage);
                                CreateEntryFromBytes(zip, sourceFile.Bytes, entryName: $"Authors/{targetFile}", compressionLevel: compressionLevel);
                            }
                        }
                    }
            }
                
            // add after authors as author mappings can change due to image compression
            CreateEntryFromString(zip, content: PackSettings.Processor.Serialize(packSettingsJson), entryName: "Settings.json", compressionLevel);

            if (packModel.PreviewsPaths.Any())
            {
                zip.CreateEntry("Previews/.nomedia");
                int previewIndex = 1;
                foreach (string previewPath in packModel.PreviewsPaths)
                {
                    string targetPath;
                    string previewExtension = Path.GetExtension(previewPath);
                    if (previewExtension == ".gif" || previewExtension == ".webp")
                    {
                        // animated .webp is not supported and we do not want compress .webp once again
                        targetPath = previewPath;
                    }
                    else
                    {
                        targetPath = IOUtils.ChooseLighterFileAndDeleteSecond(
                            previewPath,
                            _imageConverter.ConvertImageToTempWebPFile(previewPath, lossless: false)
                        );
                    }

                    zip.CreateEntryFromFile(
                        sourceFileName: targetPath,
                        entryName: $"Previews/{previewIndex++}{Path.GetExtension(targetPath)}",
                        compressionLevel
                    );
                }
            }

            int fileIndex = 1;
            foreach (PackModel.ModifiedFile modifiedFile in packModel.ModifiedFiles) {
                var (convertedFile, configFile) = _fileConverter.ConvertToTarget(modifiedFile.FileType, modifiedFile.FilePath, modifiedFile.Config).GetAwaiter().GetResult();
                string fileExt = PackUtils.GetConvertedFilesExt(modifiedFile.FileType);
                string fileName = fileIndex.ToString();
                fileIndex++;
                zip.CreateEntryFromFile(
                    sourceFileName: convertedFile,
                    entryName: $"Modified/{fileName}{fileExt}",
                    compressionLevel
                );
                if (configFile != null) {
                    zip.CreateEntryFromFile(
                        sourceFileName: configFile,
                        entryName: $"Modified/{fileName}.json",
                        compressionLevel
                    );
                }
            }
        }
    }

    private static void CreateEntryFromBytes(ZipArchive zip, byte[] content, string entryName, CompressionLevel compressionLevel)
    {
        ZipArchiveEntry entry = zip.CreateEntry(entryName, compressionLevel);
        using (Stream stream = entry.Open())
            stream.Write(content, offset: 0, count: content.Length);
    }
    
    private static void CreateEntryFromString(ZipArchive zip, string content, string entryName, CompressionLevel compressionLevel)
    {
        ZipArchiveEntry entry = zip.CreateEntry(entryName, compressionLevel);
        using (Stream stream = entry.Open())
            using (var writer = new StreamWriter(stream, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false)))
                writer.Write(content);
    }

    private static AuthorJson AuthorModelToJson(
        PackModel.Author author,
        ref int authorFileIndex,
        out bool copyIcon,
        out string? fileExtension
    )
    {
        string name = author.Name;
        string? color = author.Color == null ? null : AuthorColorToString(author.Color.Value);
        string link = author.Link;
            
        string? icon = null;
        fileExtension = null;
        if (author.Icon != null) {
            string extension = author.Icon.Type switch
            {
                ImageInfo.ImageType.Png => ".png",
                ImageInfo.ImageType.Gif => ".gif",
                _ => throw new ArgumentOutOfRangeException(nameof(author.Icon.Type), author.Icon.Type, @"Unknown type")
            };

            fileExtension = extension;
            icon = authorFileIndex + extension;
            authorFileIndex++;
            copyIcon = true;
        }
        else
        {
            copyIcon = false;
        }
            
        return new AuthorJson(
            name: name,
            color: color,
            file: icon,
            link: link,
            iconHeight: author.IconHeight
        );
    }

    private PackModel.Author JsonToAuthorModel(AuthorJson author, string authorIconsDir)
    {
        return new(
            Name: author.Name ?? string.Empty,
            Color: author.Color == null ? null : AuthorColorFromString(author.Color),
            Link: author.Link ?? string.Empty,
            Icon: author.File == null ? null : ParseAuthorIcon(Path.Combine(authorIconsDir, author.File)),
            IconHeight: author.IconHeight
        );
    }

    private static Color AuthorColorFromString(string color)
    {
        return ColorTranslator.FromHtml(color);
    }

    private static string AuthorColorToString(Color color)
    {
        return ColorTranslator.ToHtml(color);
    }
        
    private ImageInfo? ParseAuthorIcon(string iconPath)
    {
        if (!File.Exists(iconPath))
            return null;
            
        ImageInfo.ImageType iconType;
        switch (Path.GetExtension(iconPath)) {
            case ".png":
                iconType = ImageInfo.ImageType.Png;
                break;
            case ".gif":
                iconType = ImageInfo.ImageType.Gif;
                break;
            case ".webp":
                iconType = ImageInfo.ImageType.Png;
                iconPath = _imageConverter.ConvertWebPToTempPngFile(iconPath);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(iconPath), iconPath, @"Unknown extension");
        }
        return new ImageInfo(File.ReadAllBytes(iconPath), iconType);
    }
}