using System;
using System.Collections.Generic;
using System.Windows.Media;
using TerrLauncherPackCreator.Code.Enums;
using TerrLauncherPackCreator.Code.Implementations;
using TerrLauncherPackCreator.Code.Interfaces;

namespace TerrLauncherPackCreator.Code.Models
{
    public class PackModel
    {
        public class ModifiedFileInfo
        {
            public IPackFileInfo? Config { get; }
            
            public string FilePath { get; }

            public FileType FileType { get; }
            
            public ModifiedFileInfo(
                IPackFileInfo? config,
                string filePath,
                FileType fileType
            )
            {
                Config = config;
                FilePath = filePath;
                FileType = fileType;
            }
        }
        
        public PackModel(
            (string name, Color? color, string link, ImageInfo icon, int iconHeight)[] authors,
            string[] previewsPaths,
            ModifiedFileInfo[] modifiedFiles,
            int packStructureVersion,
            string iconFilePath,
            string title,
            string descriptionRussian,
            string descriptionEnglish,
            Guid guid,
            int version,
            bool isBonusPack,
            List<PredefinedPackTag> predefinedTags
        )
        {
            Authors = authors;
            PreviewsPaths = previewsPaths;
            ModifiedFiles = modifiedFiles;
            PackStructureVersion = packStructureVersion;
            IconFilePath = iconFilePath;
            Title = title;
            DescriptionRussian = descriptionRussian;
            DescriptionEnglish = descriptionEnglish;
            Guid = guid;
            Version = version;
            IsBonusPack = isBonusPack;
            PredefinedTags = predefinedTags;
        }

        public int PackStructureVersion { get; }
        
        public string IconFilePath { get; }

        public string Title { get; }

        public string DescriptionRussian { get; }

        public string DescriptionEnglish { get; }

        public Guid Guid { get; }

        public int Version { get; }

        public bool IsBonusPack { get; }

        public List<PredefinedPackTag> PredefinedTags { get; }

        public (string name, Color? color, string link, ImageInfo icon, int iconHeight)[] Authors { get; }
        
        public string[] PreviewsPaths { get; }

        public ModifiedFileInfo[] ModifiedFiles { get; }
    }
}
