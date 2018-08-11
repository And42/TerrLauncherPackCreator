using System;

namespace TerrLauncherPackCreator.Code.Models
{
    public class PackModel
    {
        public string IconFilePath { get; set; }

        public string Title { get; set; }

        public string DescriptionRussian { get; set; }

        public string DescriptionEnglish { get; set; }

        public Guid Guid { get; set; }

        public int Version { get; set; }

        public string[] PreviewsPaths { get; set; }

        public string[] ModifiedFilesPaths { get; set; }
    }
}
