using JetBrains.Annotations;
using Newtonsoft.Json;
using TerrLauncherPackCreator.Code.Interfaces;

namespace TerrLauncherPackCreator.Code.Json
{
    public class CharacterFileInfo : IPackFileInfo
    {
        [NotNull]
        [JsonProperty("result_file_name", Required = Required.Always)]
        public string ResultFileName { get; set; }

        public CharacterFileInfo()
        {
            ResultFileName = string.Empty;
        }

        public CharacterFileInfo([NotNull] string resultFileName)
        {
            ResultFileName = resultFileName;
        }
    }
}