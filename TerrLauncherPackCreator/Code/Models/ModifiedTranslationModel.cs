using System.Collections.Generic;
using CrossPlatform.Code.Utils;

namespace TerrLauncherPackCreator.Code.Models
{
    public class ModifiedTranslationModel : ModifiedFileModel
    {
        public string CurrentLanguage
        {
            get => _currentLanguage;
            set => SetProperty(ref _currentLanguage, value);
        }
        private string _currentLanguage;

        public bool IgnoreForCategory
        {
            get => _ignoreForCategory;
            set => SetProperty(ref _ignoreForCategory, value);
        }
        private bool _ignoreForCategory;

        public IReadOnlyList<string> ShortLanguages { get; }
        
        public ModifiedTranslationModel(string filePath, bool isDragDropTarget) : base(filePath, isDragDropTarget)
        {
            ShortLanguages = PackUtils.TranslationLanguages;
            _currentLanguage = ShortLanguages[0];
        }
    }
}