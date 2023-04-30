namespace TerrLauncherPackCreator.Code.Interfaces;

public interface ITempDirsProvider
{
    string GetNewDir();

    void DeleteAll();
}