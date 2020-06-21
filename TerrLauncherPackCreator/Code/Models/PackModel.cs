using System;
using System.Collections.Generic;
using System.Windows.Media;
using JetBrains.Annotations;
using TerrLauncherPackCreator.Code.Enums;
using TerrLauncherPackCreator.Code.Implementations;
using TerrLauncherPackCreator.Code.Interfaces;

namespace TerrLauncherPackCreator.Code.Models
{
    public class PackModel
    {
        public class ModifiedFileInfo
        {
            [CanBeNull]
            public IPackFileInfo Config { get; set; }
            
            [NotNull]
            public string FilePath { get; set; }

            public FileType FileType { get; set; }
            
            public ModifiedFileInfo([NotNull] string filePath)
            {
                FilePath = filePath;
            }
        }
        
        public PackModel(
            [NotNull] (string name, Color? color, string link, ImageInfo icon)[] authors,
            [NotNull] string[] previewsPaths,
            [NotNull] ModifiedFileInfo[] modifiedFiles
        )
        {
            Authors = authors;
            PreviewsPaths = previewsPaths;
            ModifiedFiles = modifiedFiles;
        }

        public int PackStructureVersion { get; set; }
        
        public string IconFilePath { get; set; }

        public string Title { get; set; }

        public string DescriptionRussian { get; set; }

        public string DescriptionEnglish { get; set; }

        public Guid Guid { get; set; }

        public int Version { get; set; }

        [NotNull]
        public List<PredefinedPackTag> PredefinedTags { get; set; }

        [NotNull]
        public (string name, Color? color, string link, ImageInfo icon)[] Authors { get; set; }
        
        [NotNull]
        public string[] PreviewsPaths { get; set; }

        [NotNull]
        public ModifiedFileInfo[] ModifiedFiles { get; set; }
    }
}
