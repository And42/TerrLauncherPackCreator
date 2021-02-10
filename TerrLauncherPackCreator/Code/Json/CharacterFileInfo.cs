using Newtonsoft.Json;
using TerrLauncherPackCreator.Code.Interfaces;

namespace TerrLauncherPackCreator.Code.Json
{
    public class CharacterFileInfo : IPackFileInfo
    {
        [JsonProperty("result_file_name", Required = Required.Always)]
        public string ResultFileName { get; set; }

        public CharacterFileInfo()
        {
            ResultFileName = string.Empty;
        }

        public CharacterFileInfo(string resultFileName)
        {
            ResultFileName = resultFileName;
        }
    }
}