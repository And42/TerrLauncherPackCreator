using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using TerrLauncherPackCreator.Code.Interfaces;
using TerrLauncherPackCreator.Code.Models;

namespace TerrLauncherPackCreator.Code.Implementations
{
    public class PackProcessor : IPackProcessor
    {
        public event Action<(string filePath, PackModel loadedPack, Exception error)> PackLoaded;

        public event Action<(PackModel pack, string targetFilePath, Exception error)> PackSaved;

        private readonly IProgressManager _progressManager;

        private readonly ConcurrentQueue<string> _loadingQueue = new ConcurrentQueue<string>();
        private readonly ConcurrentQueue<(PackModel pack, string targetFilePath)> _savingQueue = new ConcurrentQueue<(PackModel, string)>();

        public PackProcessor(IProgressManager progressManager = null)
        {
            _progressManager = progressManager;
        }

        public void LoadPackFromFile(string filePath)
        {
            if (filePath == null)
                throw new ArgumentNullException(nameof(filePath));

            _loadingQueue.Enqueue(filePath);
            StartLoadingTask();
        }

        public void SavePackToFile(PackModel pack, string targetFilePath)
        {
            if (targetFilePath == null)
                throw new ArgumentNullException(targetFilePath);

            _savingQueue.Enqueue((pack, targetFilePath));
            StartSavingTask();
        }

        private void StartLoadingTask()
        {
            Task.Run(() =>
            {
                string filePath;
                while (_loadingQueue.TryDequeue(out filePath))
                {
                    try
                    {
                        var packModel = LoadPackModelInternal(filePath);
                        PackLoaded?.Invoke((filePath, packModel, null));
                    }
                    catch (Exception ex)
                    {
                        PackLoaded?.Invoke((filePath, null, ex));
                    }
                }
            });
        }

        private void StartSavingTask()
        {
            Task.Run(() =>
            {
                (PackModel pack, string targetFilePath) item;
                while (_savingQueue.TryDequeue(out item))
                {
                    try
                    {
                        SavePackModelInternal(item.pack, item.targetFilePath);
                        PackSaved?.Invoke((item.pack, item.targetFilePath, null));
                    }
                    catch (Exception ex)
                    {
                        PackSaved?.Invoke((item.pack, item.targetFilePath, ex));
                    }
                }
            });
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
