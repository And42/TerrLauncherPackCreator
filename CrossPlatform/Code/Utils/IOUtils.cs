using System;
using System.IO;
using System.Threading;

namespace CommonLibrary.CommonUtils
{
    // ReSharper disable once InconsistentNaming
    public static class IOUtils
    {
        public static void TryDeleteFile(string filePath, int triesCount, int triesSleepMs)
        {
            if (triesCount < 1)
                throw new ArgumentOutOfRangeException(nameof(triesCount));
            if (triesSleepMs < 0)
                throw new ArgumentOutOfRangeException(nameof(triesSleepMs));

            if (!File.Exists(filePath))
                return;

            TryAction(() => File.Delete(filePath), triesCount, triesSleepMs);
        }

        public static void TryDeleteDirectory(string dirPath, int triesCount, int triesSleepMs)
        {
            if (triesCount < 1)
                throw new ArgumentOutOfRangeException(nameof(triesCount));
            if (triesSleepMs < 0)
                throw new ArgumentOutOfRangeException(nameof(triesSleepMs));

            if (!Directory.Exists(dirPath))
                return;

            TryAction(() => Directory.Delete(dirPath, true), triesCount, triesSleepMs);
        }

        public static void EnsureDirExists(string dir)
        {
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }
        
        public static void EnsureParentDirExists(string filePath)
        {
            string? parentDir = Path.GetDirectoryName(filePath);
            if (parentDir != null)
                EnsureDirExists(parentDir);
        }

        public static string ChooseLighterFileAndDeleteSecond(string first, string second)
        {
            if (new FileInfo(first).Length > new FileInfo(second).Length)
                return second;
            
            File.Delete(second);
            return first;
        }

        private static void TryAction(Action action, int triesCount, int triesSleepMs)
        {
            int currentTry = 1;
            while (true)
            {
                try
                {
                    action();
                    break;
                }
                catch (Exception)
                {
                    if (currentTry == triesCount)
                        throw;

                    currentTry++;
                    Thread.Sleep(triesSleepMs);
                }
            }
        }
    }
}
