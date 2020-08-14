using Newtonsoft.Json;
using TerrLauncherPackCreator.Code.Interfaces;

namespace TerrLauncherPackCreator.Code.Json
{
    public class TextureFileInfo : IPackFileInfo
    {
        public enum TextureType
        {
            General = 0,
            Item = 1,
            Npc = 2,
            Buff = 3,
            Extra = 4
        }
        
        [JsonProperty("type")]
        public TextureType Type { get; set; }
        
        [JsonProperty("entry_name")]
        public string EntryName { get; set; }

        [JsonProperty("element_id")]
        public int ElementId { get; set; }

        [JsonProperty("animated")]
        public bool Animated { get; set; }

        [JsonProperty("animate_in_gui")]
        public bool AnimateInGui { get; set; } = true;

        [JsonProperty("number_of_vertical_frames")]
        public int NumberOfVerticalFrames { get; set; } = 1;

        [JsonProperty("number_of_horizontal_frames")]
        public int NumberOfHorizontalFrames { get; set; } = 1;

        [JsonProperty("milliseconds_per_frame")]
        public int MillisecondsPerFrame { get; set; } = 100;

        [JsonProperty("apply_original_size")]
        public bool ApplyOriginalSize { get; set; } = true;
    }
}