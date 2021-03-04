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

        public IReadOnlyList<string> ShortLanguages { get; }
        
        public ModifiedTranslationModel(string filePath, bool isDragDropTarget) : base(filePath, isDragDropTarget)
        {
            ShortLanguages = PackUtils.TranslationLanguages;
            _currentLanguage = ShortLanguages[0];
        }
    }
}