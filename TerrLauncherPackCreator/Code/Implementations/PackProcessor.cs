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
using TerrLauncherPackCreator.Code.Interfaces;
using TerrLauncherPackCreator.Code.Json;
using TerrLauncherPackCreator.Code.Models;
using TerrLauncherPackCreator.Code.Utils;

namespace TerrLauncherPackCreator.Code.Implementations
{
    public class PackProcessor : IPackProcessor
    {
        private static readonly ISet<string> PreviewExtensions = new HashSet<string> {".jpg", ".png", ".gif"};
        private const int PackProcessingTries = 20;
        private const int PackProcessingSleepMs = 500;

        public event Action<string> PackLoadingStarted;

        public event Action<(string filePath, PackModel loadedPack, Exception error)> PackLoaded;

        public event Action<(PackModel pack, string targetFilePath)> PackSavingStarted;

        public event Action<(PackModel pack, string targetFilePath, Exception error)> PackSaved;

        private readonly IProgressManager _loadProgressManager;
        private readonly IProgressManager _saveProgressManager;

        private readonly object _loadingLock = new object();
        private readonly object _savingLock = new object();

        public PackProcessor(
            IProgressManager loadProgressManager,
            IProgressManager saveProgressManager
        )
        {
            _loadProgressManager = loadProgressManager;
            _saveProgressManager = saveProgressManager;

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
            // todo: add loading existing packs
            throw new NotSupportedException();
            
            if (filePath == null)
                throw new ArgumentNullException(nameof(filePath));

            Task.Run(() =>
            {
                try
                {
                    PackLoadingStarted?.Invoke(filePath);
                    var packModel = LoadPackModelInternal(filePath);
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

        private static PackModel LoadPackModelInternal(string filePath)
        {
            // todo: add loading packs support
            throw new NotSupportedException();
            
            // todo: implement the ability to process multiple packs with the same names simultaneously

//            string packExt = Path.GetExtension(filePath);
//
//            Debug.Assert(PackUtils.PacksInfo.Select(it => it.packExt).Contains(packExt));
//
//            var packTypeInfo = PackUtils.PacksInfo.First(it => it.packExt == packExt);
//
//            string targetFolderPath = Path.Combine(
//                ApplicationDataUtils.PathToTempFolder,
//                Path.GetFileNameWithoutExtension(filePath) ?? "undefined"
//            );
//
//            IOUtils.TryDeleteDirectory(targetFolderPath, PackProcessingTries, PackProcessingSleepMs);
//
//            using (var zip = ZipFile.Read(filePath))
//            {
//                zip.ExtractAll(targetFolderPath);
//            }
//
//            string packSettingsFile = Path.Combine(targetFolderPath, "Settings.json");
//            string packIconGif = Path.Combine(targetFolderPath, "Icon.gif");
//            string packIconPng = Path.Combine(targetFolderPath, "Icon.png");
//            string packPreviewsFolder = Path.Combine(targetFolderPath, "Previews");
//            string packModifiedFilesFolder = Path.Combine(targetFolderPath, "Modified");
//
//            PackSettings packSettings = JsonConvert.DeserializeObject<PackSettings>(File.ReadAllText(packSettingsFile, Encoding.UTF8));
//
//            string packIconFile = null;
//            if (File.Exists(packIconGif))
//                packIconFile = packIconGif;
//            else if (File.Exists(packIconPng))
//                packIconFile = packIconPng;
//
//            string[] previewsPaths = 
//                Directory.Exists(packPreviewsFolder)
//                    ? Directory.EnumerateFiles(packPreviewsFolder).Where(it => PreviewExtensions.Contains(Path.GetExtension(it))).ToArray()
//                    : null;
//
//            string[] modifiedFilesPaths =
//                Directory.Exists(packModifiedFilesFolder)
//                    ? Directory.GetFiles(packModifiedFilesFolder, "*" + packTypeInfo.packFilesExt)
//                    : null;
//
//            var packModel = new PackModel
//            {
//                PackType = packTypeInfo.packType,
//                IconFilePath = packIconFile,
//                Title = packSettings.Title,
//                DescriptionEnglish = packSettings.DescriptionEnglish,
//                DescriptionRussian = packSettings.DescriptionRussian,
//                Version = packSettings.Version,
//                Guid = packSettings.Guid,
//                PreviewsPaths = previewsPaths,
//                ModifiedFilesPaths = modifiedFilesPaths
//            };
//
//            return packModel;
        }

        private static void SavePackModelInternal(PackModel packModel, string filePath)
        {
            var authorsMappings = new List<(byte[] sourceFile, string targetFile, string json)>();
            int authorFileIndex = 1;
            foreach (var author in packModel.Authors)
            {
                string json = AuthorModelToString(author, ref authorFileIndex, out bool copyIcon);
                authorsMappings.Add((copyIcon ? author.iconBytes : null, copyIcon ? $"{authorFileIndex - 1}.png" : null, json));
            }
            var packSettingsJson = new PackSettings
            {
                TerrariaStructureVersion = packModel.TerrariaStructureVersion,
                PackStructureVersion = packModel.PackStructureVersion,
                Title = packModel.Title,
                DescriptionEnglish = packModel.DescriptionEnglish,
                DescriptionRussian = packModel.DescriptionRussian,
                Version = packModel.Version,
                Guid = packModel.Guid,
                Authors = string.Join("<->", authorsMappings.Select(it => it.json))
            };

            IOUtils.TryDeleteFile(filePath, PackProcessingTries, PackProcessingSleepMs);

            using (var zip = new ZipFile(filePath, Encoding.UTF8))
            {
                zip.AddEntry(".nomedia", new byte[0]);
                zip.AddEntry("Settings.json", JsonUtils.Serialize(packSettingsJson), Encoding.UTF8);

                if (!string.IsNullOrEmpty(packModel.IconFilePath))
                    zip.AddFile(packModel.IconFilePath).FileName = $"Icon{Path.GetExtension(packModel.IconFilePath)}";

                if (authorsMappings.Any())
                {
                    zip.AddEntry("Authors/.nomedia", new byte[0]);
                    foreach (var (sourceFile, targetFile, _) in authorsMappings)
                        if (sourceFile != null)
                            zip.AddEntry($"Authors/{targetFile}", sourceFile);
                }

                if (packModel.PreviewsPaths.Any())
                {
                    zip.AddEntry("Previews/.nomedia", new byte[0]);
                    int previewIndex = 1;
                    foreach (string previewPath in packModel.PreviewsPaths)
                        zip.AddFile(previewPath).FileName =
                            $"Previews/{previewIndex++}{Path.GetExtension(previewPath)}";
                }

                int fileIndex = 1;
                foreach (PackModel.ModifiedFileInfo modifiedFileInfo in packModel.ModifiedFiles)
                {
                    string fileName = fileIndex.ToString();
                    fileIndex++;
                    zip.AddFile(modifiedFileInfo.FilePath).FileName = $"Modified/{fileName}{Path.GetExtension(modifiedFileInfo.FilePath)}";
                    if (modifiedFileInfo.Config != null)
                    {
                        zip.AddEntry(
                            $"Modified/{fileName}.json",
                            JsonConvert.SerializeObject(modifiedFileInfo.Config, Formatting.Indented)
                        );
                    }
                }

                zip.Save();
            }
        }

        [NotNull]
        private static string AuthorModelToString((string name, Color? color, string link, byte[] iconBytes) author, ref int authorFileIndex, out bool copyIcon)
        {
            var parts = new List<string>();

            if (!string.IsNullOrEmpty(author.name))
                parts.Add("name=" + author.name);
            if (author.color.HasValue)
                parts.Add("color=" + author.color);
            if (!string.IsNullOrEmpty(author.link))
                parts.Add("link=" + author.link);
            if (author.iconBytes != null)
            {
                parts.Add($"file={authorFileIndex.ToString()}.png");
                authorFileIndex++;
                copyIcon = true;
            }
            else
            {
                copyIcon = false;
            }

            return string.Join("|", parts);
        }
    }
}
