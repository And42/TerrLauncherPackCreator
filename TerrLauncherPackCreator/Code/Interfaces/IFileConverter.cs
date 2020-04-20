using System.Threading.Tasks;
using JetBrains.Annotations;
using TerrLauncherPackCreator.Code.Enums;
using TerrLauncherPackCreator.Code.Models;

namespace TerrLauncherPackCreator.Code.Interfaces
{
    public interface IFileConverter
    {
        [NotNull]
        Task<(string convertedFile, string configFile)> ConvertToTarget(FileType fileType, [NotNull] string sourceFile, [CanBeNull] IPackFileInfo fileInfo);
        
        [NotNull]
        Task<(string sourceFile, IPackFileInfo fileInfo)> ConvertToSource(FileType fileType, [NotNull] string targetFile, [CanBeNull] string configFile);
    }
}