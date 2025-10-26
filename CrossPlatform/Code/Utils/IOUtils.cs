using System;
using System.IO;
using System.Threading;

namespace CrossPlatform.Code.Utils;

// ReSharper disable once InconsistentNaming
public static class IOUtils
{
    public static void TryDeleteFile(string filePath, int triesCount, int triesSleepMs)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(triesCount, 1);
        ArgumentOutOfRangeException.ThrowIfNegative(triesSleepMs);

        if (!File.Exists(filePath))
            return;

        TryAction(() => File.Delete(filePath), triesCount, triesSleepMs);
    }

    public static void TryDeleteDirectory(string dirPath, int triesCount, int triesSleepMs)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(triesCount, 1);
        ArgumentOutOfRangeException.ThrowIfNegative(triesSleepMs);

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

    public static void CopyFile(string source, string target, bool overwrite = false)
    {
        EnsureParentDirExists(target);
        File.Copy(source, target, overwrite: overwrite);
    }
        
    public static void CopyDirectory(string source, string target, bool overwriteFiles = false)
    {
        EnsureDirExists(target);
            
        foreach (string directory in Directory.EnumerateDirectories(source, "*", SearchOption.AllDirectories))
        {
            string newDir = Path.Combine(target, directory[(source.Length + 1)..]);
            EnsureDirExists(newDir);
        }

        foreach (string file in Directory.EnumerateFiles(source, "*", SearchOption.AllDirectories))
        {
            string newFile = Path.Combine(target, file[(source.Length + 1)..]);
            File.Copy(file, newFile, overwrite: overwriteFiles);
        }
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