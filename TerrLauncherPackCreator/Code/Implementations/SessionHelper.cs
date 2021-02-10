using CommonLibrary.CommonUtils;
using TerrLauncherPackCreator.Code.Interfaces;

namespace TerrLauncherPackCreator.Code.Implementations
{
    public class SessionHelper : ISessionHelper
    {
        public static readonly SessionHelper Instance = new();

        private SessionHelper() {}

        public string GenerateNonExistentDirPath()
        {
            return ApplicationDataUtils.GenerateNonExistentDirPath();
        }

        public string GenerateNonExistentFilePath(string? extension = null)
        {
            return ApplicationDataUtils.GenerateNonExistentFilePath();
        }
    }
}