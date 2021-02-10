using System.Collections.Generic;
using JetBrains.Annotations;
using TerrLauncherPackCreator.Code.Utils;

namespace TerrLauncherPackCreator.Code.Models
{
    public class ModifiedTranslationModel : ModifiedFileModel
    {
        [NotNull]
        public string CurrentLanguage
        {
            get => _currentLanguage;
            set => SetProperty(ref _currentLanguage, value);
        }
        [NotNull]
        private string _currentLanguage;

        [NotNull]
        public IReadOnlyList<string> ShortLanguages { get; }
        
        public ModifiedTranslationModel([NotNull] string filePath, bool isDragDropTarget) : base(filePath, isDragDropTarget)
        {
            ShortLanguages = PackUtils.TranslationLanguages;
            _currentLanguage = ShortLanguages[0];
        }
    }
}