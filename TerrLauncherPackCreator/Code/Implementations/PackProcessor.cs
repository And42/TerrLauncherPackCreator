using System;
using System.Threading.Tasks;
using TerrLauncherPackCreator.Code.Interfaces;
using TerrLauncherPackCreator.Code.Models;

namespace TerrLauncherPackCreator.Code.Implementations
{
    public class PackProcessor : IPackProcessor
    {
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

        private PackModel LoadPackModelInternal(string filePath)
        {
            throw new NotImplementedException();
        }

        private void SavePackModelInternal(PackModel packModel, string filePath)
        {
            throw new NotImplementedException();
        }
    }
}
