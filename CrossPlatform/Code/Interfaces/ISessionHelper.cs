namespace CrossPlatform.Code.Interfaces;

public interface ISessionHelper
{
    string GenerateNonExistentDirPath();

    string GenerateNonExistentFilePath(string? extension = null);
}