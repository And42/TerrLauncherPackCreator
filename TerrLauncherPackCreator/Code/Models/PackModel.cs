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
            public IPackFileInfo Config { get; }
            
            [NotNull]
            public string FilePath { get; }

            public FileType FileType { get; }
            
            public ModifiedFileInfo(
                [CanBeNull] IPackFileInfo config,
                [NotNull] string filePath,
                FileType fileType
            )
            {
                Config = config;
                FilePath = filePath;
                FileType = fileType;
            }
        }
        
        public PackModel(
            [NotNull] (string name, Color? color, string link, ImageInfo icon, int iconHeight)[] authors,
            [NotNull] string[] previewsPaths,
            [NotNull] ModifiedFileInfo[] modifiedFiles,
            int packStructureVersion,
            string iconFilePath,
            string title,
            string descriptionRussian,
            string descriptionEnglish,
            Guid guid,
            int version,
            bool isBonusPack,
            [NotNull] List<PredefinedPackTag> predefinedTags
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

        [NotNull]
        public List<PredefinedPackTag> PredefinedTags { get; }

        [NotNull]
        public (string name, Color? color, string link, ImageInfo icon, int iconHeight)[] Authors { get; }
        
        [NotNull]
        public string[] PreviewsPaths { get; }

        [NotNull]
        public ModifiedFileInfo[] ModifiedFiles { get; }
    }
}
