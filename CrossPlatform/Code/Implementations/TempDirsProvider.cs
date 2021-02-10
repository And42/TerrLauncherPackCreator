using System.IO;
using CommonLibrary.CommonUtils;
using JetBrains.Annotations;
using TerrLauncherPackCreator.Code.Interfaces;

namespace TerrLauncherPackCreator.Code.Implementations
{
    public class TempDirsProvider : ITempDirsProvider
    {
        [NotNull]
        private readonly string _initialDir;

        public TempDirsProvider([NotNull] string initialDir)
        {
            if (!Directory.Exists(initialDir))
                Directory.CreateDirectory(initialDir);
            
            _initialDir = initialDir;
        }
        
        [NotNull]
        public string GetNewDir()
        {
            for (int i = 1;; i++)
            {
                string newDir = Path.Combine(_initialDir, i.ToString());
                if (!Directory.Exists(newDir) && !File.Exists(newDir))
                {
                    Directory.CreateDirectory(newDir);
                    return newDir;
                }
            }
        }

        public void DeleteAll()
        {
            if (!Directory.Exists(_initialDir))
                return;
            
            Directory.EnumerateDirectories(_initialDir).ForEach(dir => Directory.Delete(dir, recursive: true));
            Directory.EnumerateFiles(_initialDir).ForEach(File.Delete);
        }
    }
}