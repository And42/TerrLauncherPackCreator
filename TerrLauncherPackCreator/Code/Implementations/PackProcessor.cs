using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ionic.Zip;
using Newtonsoft.Json;
using TerrLauncherPackCreator.Code.Interfaces;
using TerrLauncherPackCreator.Code.Json;
using TerrLauncherPackCreator.Code.Models;
using TerrLauncherPackCreator.Code.Utils;

namespace TerrLauncherPackCreator.Code.Implementations
{
    public class PackProcessor : IPackProcessor
    {
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
            string packExt = Path.GetExtension(filePath);

            Debug.Assert(PackUtils.PacksInfo.Select(it => it.packExt).Contains(packExt));

            var packTypeInfo = PackUtils.PacksInfo.First(it => it.packExt == packExt);

            string targetFolderPath = Path.Combine(
                ApplicationDataUtils.PathToTempFolder,
                Path.GetFileNameWithoutExtension(filePath) ?? "undefined"
            );

            if (Directory.Exists(targetFolderPath))
            {
                int currentTry = 1;
                while (true)
                {
                    try
                    {
                        Directory.Delete(targetFolderPath, true);
                        break;
                    }
                    catch (Exception)
                    {
                        if (currentTry == PackProcessingTries)
                            throw;

                        currentTry++;
                        Thread.Sleep(PackProcessingSleepMs);
                    }
                }
            }

            using (var zip = new ZipFile(filePath))
            {
                zip.ExtractAll(targetFolderPath);
            }

            string packSettingsFile = Path.Combine(targetFolderPath, "Settings.json");
            string packIconFile = Path.Combine(targetFolderPath, "Icon.png");
            string packPreviewsFolder = Path.Combine(targetFolderPath, "Previews");
            string packModifiedFilesFolder = Path.Combine(targetFolderPath, "Modified");

            PackSettings packSettings = JsonConvert.DeserializeObject<PackSettings>(File.ReadAllText(packSettingsFile, Encoding.UTF8));

            if (!File.Exists(packIconFile))
                packIconFile = null;

            string[] previewsPaths = 
                Directory.Exists(packPreviewsFolder)
                    ? Directory.GetFiles(packPreviewsFolder, "*.jpg")
                    : null;

            string[] modifiedFilesPaths =
                Directory.Exists(packModifiedFilesFolder)
                    ? Directory.GetFiles(packModifiedFilesFolder, "*" + packTypeInfo.packFilesExt)
                    : null;

            var packModel = new PackModel
            {
                PackType = packTypeInfo.packType,
                IconFilePath = packIconFile,
                Title = packSettings.Title,
                DescriptionEnglish = packSettings.DescriptionEnglish,
                DescriptionRussian = packSettings.DescriptionRussian,
                Version = packSettings.Version,
                Guid = packSettings.Guid,
                PreviewsPaths = previewsPaths,
                ModifiedFilesPaths = modifiedFilesPaths
            };

            return packModel;
        }

        private static void SavePackModelInternal(PackModel packModel, string filePath)
        {
            throw new NotImplementedException();
        }
    }
}
