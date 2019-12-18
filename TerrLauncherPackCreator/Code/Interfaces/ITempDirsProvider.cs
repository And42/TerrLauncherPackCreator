using JetBrains.Annotations;

namespace TerrLauncherPackCreator.Code.Interfaces
{
    public interface ITempDirsProvider
    {
        [NotNull]
        string GetNewDir();

        void DeleteAll();
    }
}