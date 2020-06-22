using JetBrains.Annotations;
using Newtonsoft.Json;
using TerrLauncherPackCreator.Code.Interfaces;

namespace TerrLauncherPackCreator.Code.Json
{
    public class MapFileInfo : IPackFileInfo
    {
        [NotNull]
        [JsonProperty("result_file_name", Required = Required.Always)]
        public string ResultFileName { get; set; }

        public MapFileInfo()
        {
            ResultFileName = string.Empty;
        }

        public MapFileInfo([NotNull] string resultFileName)
        {
            ResultFileName = resultFileName;
        }
    }
}