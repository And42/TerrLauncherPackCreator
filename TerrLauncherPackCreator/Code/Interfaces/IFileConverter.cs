using System.Threading.Tasks;
using TerrLauncherPackCreator.Code.Enums;

namespace TerrLauncherPackCreator.Code.Interfaces
{
    public interface IFileConverter
    {
        Task<(string convertedFile, string? configFile)> ConvertToTarget(
            FileType fileType,
            string sourceFile,
            IPackFileInfo? fileInfo
        );

        Task<(string sourceFile, IPackFileInfo? fileInfo)> ConvertToSource(
            int packStructureVersion,
            FileType fileType,
            string targetFile,
            string? configFile
        );
    }
}