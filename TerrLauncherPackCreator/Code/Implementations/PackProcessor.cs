using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using CommonLibrary.CommonUtils;
using Ionic.Zip;
using JetBrains.Annotations;
using Newtonsoft.Json;
using TerrLauncherPackCreator.Code.Enums;
using TerrLauncherPackCreator.Code.Interfaces;
using TerrLauncherPackCreator.Code.Json;
using TerrLauncherPackCreator.Code.Models;
using TerrLauncherPackCreator.Code.Utils;

namespace TerrLauncherPackCreator.Code.Implementations
{
    public class PackProcessor : IPackProcessor
    {
        private static readonly ISet<string> PreviewExtensions = new HashSet<string> {".jpg", ".png", ".gif", ".webp"};
        private const int PackProcessingTries = 20;
        private const int PackProcessingSleepMs = 500;

        public event Action<string> PackLoadingStarted;

        public event Action<(string filePath, PackModel loadedPack, Exception error)> PackLoaded;

        public event Action<(PackModel pack, string targetFilePath)> PackSavingStarted;

        public event Action<(PackModel pack, string targetFilePath, Exception error)> PackSaved;

        private readonly IProgressManager _loadProgressManager;
        private readonly IProgressManager _saveProgressManager;
        private readonly IFileConverter _fileConverter;

        private readonly object _loadingLock = new object();
        private readonly object _savingLock = new object();

        public PackProcessor(
            IProgressManager loadProgressManager,
            IProgressManager saveProgressManager,
            IFileConverter fileConverter
        )
        {
            _loadProgressManager = loadProgressManager;
            _saveProgressManager = saveProgressManager;
            _fileConverter = fileConverter;

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
                _loadProgressManager.RemainingFilesCount++;
            }
        }

        private void OnPackLoaded((string filePath, PackModel loadedPack, Exception error) item)
        {
            lock (_loadingLock)
            {
                _loadProgressManager.RemainingFilesCount--;
            }
        }

        private void OnPackSavingStarted((PackModel pack, string targetFilePath) item)
        {
            lock (_savingLock)
            {
                _saveProgressManager.RemainingFilesCount++;
            }
        }

        private void OnPackSaved((PackModel pack, string targetFilePath, Exception error) item)
        {
            lock (_savingLock)
            {
                _saveProgressManager.RemainingFilesCount--;
            }
        }

        private async Task<PackModel> LoadPackModelInternal(string filePath)
        {
            string targetFolderPath = ApplicationDataUtils.GenerateNonExistentDirPath();
            using (var zip = ZipFile.Read(filePath))
                zip.ExtractAll(targetFolderPath);

            string packSettingsFile = Path.Combine(targetFolderPath, "Settings.json");
            string packIconGif = Path.Combine(targetFolderPath, "Icon.gif");
            string packIconPng = Path.Combine(targetFolderPath, "Icon.png");
            string packIconWebP = Path.Combine(targetFolderPath, "Icon.webp");
            string packPreviewsFolder = Path.Combine(targetFolderPath, "Previews");
            string packAuthorsFolder = Path.Combine(targetFolderPath, "Authors");
            string packModifiedFilesFolder = Path.Combine(targetFolderPath, "Modified");

            PackSettings packSettings = JsonConvert.DeserializeObject<PackSettings>(File.ReadAllText(packSettingsFile, Encoding.UTF8));

            string packIconFile = null;
            if (File.Exists(packIconGif))
                packIconFile = packIconGif;
            else if (File.Exists(packIconPng))
                packIconFile = packIconPng;
            else if (File.Exists(packIconWebP))
                packIconFile = ImageUtils.ConvertWebPToTempPngFile(packIconWebP);

            string[] previewsPaths = 
                Directory.Exists(packPreviewsFolder)
                    ? Directory.EnumerateFiles(packPreviewsFolder)
                        .Where(it => PreviewExtensions.Contains(Path.GetExtension(it)))
                        .ToArray()
                    : new string[0];

            string[] modifiedFileExts = PackUtils.PacksInfo.Select(it => it.convertedFilesExt).ToArray();
            string[] modifiedFilesPaths =
                Directory.Exists(packModifiedFilesFolder)
                    ? Directory.EnumerateFiles(packModifiedFilesFolder)
                        .Where(it => modifiedFileExts.Contains(Path.GetExtension(it)))
                        .ToArray()
                    : new string[0];

            var authors = packSettings.Authors.Split(new[] {"<->"}, StringSplitOptions.RemoveEmptyEntries)
                .ConvertAll(it => StringToAuthorModel(it, packAuthorsFolder));

            var modifiedFiles = new List<PackModel.ModifiedFileInfo>();
            foreach (string modifiedFile in modifiedFilesPaths)
            {
                string configFile = Path.ChangeExtension(modifiedFile, PackUtils.PackFileConfigExtension);
                if (!File.Exists(configFile)) {
                    configFile = null;
                }

                string fileExt = Path.GetExtension(modifiedFile);
                FileType fileType = PackUtils.PacksInfo.First(it => it.convertedFilesExt == fileExt).fileType;
                var (sourceFile, fileInfo) = await _fileConverter.ConvertToSource(fileType, modifiedFile, configFile);
                modifiedFiles.Add(new PackModel.ModifiedFileInfo(
                    config: fileInfo,
                    filePath: sourceFile,
                    fileType: fileType
                ));
            }

            return new PackModel(
                authors: authors,
                previewsPaths: previewsPaths,
                modifiedFiles: modifiedFiles.ToArray(),
                packStructureVersion: packSettings.PackStructureVersion,
                iconFilePath: packIconFile,
                title: packSettings.Title,
                descriptionRussian: packSettings.DescriptionRussian,
                descriptionEnglish: packSettings.DescriptionEnglish,
                guid: packSettings.Guid,
                version: packSettings.Version,
                isBonusPack: packSettings.IsBonus,
                predefinedTags: packSettings.PredefinedTags?.ToList() ?? new List<PredefinedPackTag>(0)
            );
        }

        private void SavePackModelInternal(PackModel packModel, string filePath)
        {
            var authorsMappings = new List<(ImageInfo sourceFile, string targetFile, string json)>();
            int authorFileIndex = 1;
            foreach (var author in packModel.Authors) {
                string fileExtension;
                string json = AuthorModelToString(author, ref authorFileIndex, out bool copyIcon, out fileExtension);
                authorsMappings.Add((copyIcon ? author.icon : null, copyIcon ? $"{authorFileIndex - 1}{fileExtension}" : null, json));
            }
            var packSettingsJson = new PackSettings(
                packStructureVersion: packModel.PackStructureVersion,
                title: packModel.Title,
                descriptionEnglish: packModel.DescriptionEnglish,
                descriptionRussian: packModel.DescriptionRussian,
                version: packModel.Version,
                guid: packModel.Guid,
                authors: string.Join("<->", authorsMappings.Select(it => it.json)),
                predefinedTags: packModel.PredefinedTags.ToList(),
                isBonus: packModel.IsBonusPack,
                // todo: change when new types are available
                bonusType: BonusType.OldVersionOwners
            );

            IOUtils.TryDeleteFile(filePath, PackProcessingTries, PackProcessingSleepMs);

            using (var zip = new ZipFile(filePath, Encoding.UTF8))
            {
                zip.AddEntry(".nomedia", new byte[0]);
                zip.AddEntry("Settings.json", JsonUtils.Serialize(packSettingsJson), Encoding.UTF8);

                if (!string.IsNullOrEmpty(packModel.IconFilePath))
                {
                    string targetPath = Path.GetExtension(packModel.IconFilePath) == ".gif"
                        ? packModel.IconFilePath
                        : IOUtils.ChooseLighterFileAndDeleteSecond(
                            packModel.IconFilePath,
                            ImageUtils.ConvertImageToTempWebPFile(packModel.IconFilePath, lossless: true)
                        );
                    zip.AddFile(targetPath).FileName = $"Icon{Path.GetExtension(targetPath)}";
                }

                if (authorsMappings.Any())
                {
                    zip.AddEntry("Authors/.nomedia", new byte[0]);
                    foreach (var (sourceFile, targetFile, _) in authorsMappings)
                        if (sourceFile != null)
                        {
                            if (sourceFile.Type == ImageInfo.ImageType.Gif)
                            {
                                zip.AddEntry($"Authors/{targetFile}", sourceFile.Bytes);
                            }
                            else
                            {
                                string webPImage = ImageUtils.ConvertImageToTempWebPFile(sourceFile.Bytes, lossless: true);
                                if (new FileInfo(webPImage).Length < sourceFile.Bytes.Length)
                                {
                                    string compressedFileName = $"Authors/{Path.ChangeExtension(targetFile, ".webp")}";
                                    zip.AddFile(webPImage).FileName = compressedFileName;
                                }
                                else
                                {
                                    File.Delete(webPImage);
                                    zip.AddEntry($"Authors/{targetFile}", sourceFile.Bytes);
                                }
                            }
                        }
                }

                if (packModel.PreviewsPaths.Any())
                {
                    zip.AddEntry("Previews/.nomedia", new byte[0]);
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
                                ImageUtils.ConvertImageToTempWebPFile(previewPath, lossless: false)
                            );
                        }

                        zip.AddFile(targetPath).FileName =
                            $"Previews/{previewIndex++}{Path.GetExtension(targetPath)}";
                    }
                }

                int fileIndex = 1;
                foreach (PackModel.ModifiedFileInfo modifiedFile in packModel.ModifiedFiles) {
                    var (convertedFile, configFile) = _fileConverter.ConvertToTarget(modifiedFile.FileType, modifiedFile.FilePath, modifiedFile.Config).GetAwaiter().GetResult();
                    string fileExt = PackUtils.GetConvertedFilesExt(modifiedFile.FileType);
                    string fileName = fileIndex.ToString();
                    fileIndex++;
                    zip.AddFile(convertedFile).FileName = $"Modified/{fileName}{fileExt}";
                    if (configFile != null) {
                        zip.AddFile(configFile).FileName = $"Modified/{fileName}.json";
                    }
                }

                zip.Save();
            }
        }

        [NotNull]
        private static string AuthorModelToString((string name, Color? color, string link, ImageInfo icon) author, ref int authorFileIndex, out bool copyIcon, [CanBeNull] out string fileExtension) {
            fileExtension = null;
            var parts = new List<string>();

            if (!string.IsNullOrEmpty(author.name))
                parts.Add("name=" + author.name);
            if (author.color.HasValue)
                parts.Add("color=" + author.color); // #FF00FF00
            if (!string.IsNullOrEmpty(author.link))
                parts.Add("link=" + author.link);
            if (author.icon != null) {
                string extension;
                switch (author.icon.Type) {
                    case ImageInfo.ImageType.Png:
                        extension = ".png";
                        break;
                    case ImageInfo.ImageType.Gif:
                        extension = ".gif";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(author.icon.Type), author.icon.Type, @"Unknown type");
                }

                fileExtension = extension;
                parts.Add($"file={authorFileIndex.ToString()}{extension}");
                authorFileIndex++;
                copyIcon = true;
            }
            else
            {
                copyIcon = false;
            }

            return string.Join("|", parts);
        }

        [NotNull]
        private static (string name, Color? color, string link, ImageInfo icon) StringToAuthorModel(string author, string authorIconsDir)
        {
            string[] parts = author.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
            string name = null;
            Color? color = null;
            string link = null;
            ImageInfo icon = null;

            foreach (string part in parts)
            {
                string[] keyValue = part.Split('=');
                if (keyValue.Length != 2)
                    continue;
                
                switch (keyValue[0])
                {
                    case "name":
                        name = keyValue[1];
                        break;
                    case "color":
                        color = (Color) ColorConverter.ConvertFromString(keyValue[1]);
                        break;
                    case "link":
                        link = keyValue[1];
                        break;
                    case "file":
                        string authorIcon = Path.Combine(authorIconsDir, keyValue[1]);
                        if (File.Exists(authorIcon)) {
                            ImageInfo.ImageType iconType;
                            switch (Path.GetExtension(authorIcon)) {
                                case ".png":
                                    iconType = ImageInfo.ImageType.Png;
                                    break;
                                case ".gif":
                                    iconType = ImageInfo.ImageType.Gif;
                                    break;
                                case ".webp":
                                    iconType = ImageInfo.ImageType.Png;
                                    authorIcon = ImageUtils.ConvertWebPToTempPngFile(authorIcon);
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException(nameof(authorIcon), authorIcon, @"Unknown extension");
                            }
                            icon = new ImageInfo(File.ReadAllBytes(authorIcon), iconType);
                        }

                        break;
                }
            }
            
            return (name, color, link, icon);
        }
    }
}
