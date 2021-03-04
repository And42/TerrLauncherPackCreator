using System;
using System.Collections.Generic;
using System.Drawing;
using CrossPlatform.Code.Enums;
using CrossPlatform.Code.Implementations;
using CrossPlatform.Code.Interfaces;

namespace CrossPlatform.Code.Models
{
    public record PackModel(
        int PackStructureVersion,
        string IconFilePath,
        string Title,
        string DescriptionRussian,
        string DescriptionEnglish,
        Guid Guid,
        int Version,
        bool IsBonusPack,
        IReadOnlyList<PredefinedPackTag> PredefinedTags,
        IReadOnlyList<PackModel.Author> Authors,
        IReadOnlyList<string> PreviewsPaths,
        IReadOnlyList<PackModel.ModifiedFile> ModifiedFiles
    )
    {
        public record Author(
            string Name,
            Color? Color,
            string Link,
            ImageInfo? Icon,
            int IconHeight
        );
        
        public record ModifiedFile(
            IPackFileInfo? Config,
            string FilePath,
            FileType FileType
        );
    }
}
