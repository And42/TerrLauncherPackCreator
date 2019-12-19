using System.Threading.Tasks;
using JetBrains.Annotations;
using TerrLauncherPackCreator.Code.Enums;

namespace TerrLauncherPackCreator.Code.Interfaces
{
    public interface IFileConverter
    {
        [NotNull]
        Task<string> ConvertToTarget(FileType fileType, [NotNull] string sourceFile, [NotNull] string packTempDir);
    }
}