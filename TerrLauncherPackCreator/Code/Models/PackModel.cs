using System;
using System.Windows.Media;
using JetBrains.Annotations;

namespace TerrLauncherPackCreator.Code.Models
{
    public class PackModel
    {
        public PackModel(
            [NotNull] (string name, Color? color, string link, string iconPath)[] authors,
            [NotNull] string[] previewsPaths,
            [NotNull] string[] modifiedFilesPaths
        )
        {
            Authors = authors;
            PreviewsPaths = previewsPaths;
            ModifiedFilesPaths = modifiedFilesPaths;
        }

        public int TerrariaStructureVersion { get; set; }
        
        public string IconFilePath { get; set; }

        public string Title { get; set; }

        public string DescriptionRussian { get; set; }

        public string DescriptionEnglish { get; set; }

        public Guid Guid { get; set; }

        public int Version { get; set; }

        [NotNull]
        public (string name, Color? color, string link, string iconPath)[] Authors { get; set; }
        
        [NotNull]
        public string[] PreviewsPaths { get; set; }

        [NotNull]
        public string[] ModifiedFilesPaths { get; set; }
    }
}
