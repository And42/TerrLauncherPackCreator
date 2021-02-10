using CommonLibrary.CommonUtils;
using TerrLauncherPackCreator.Code.Interfaces;

namespace TerrLauncherPackCreator.Code.Implementations
{
    public class SessionHelper : ISessionHelper
    {
        public string GenerateNonExistentDirPath()
        {
            return ApplicationDataUtils.GenerateNonExistentDirPath();
        }
    }
}