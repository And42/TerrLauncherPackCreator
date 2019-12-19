using JetBrains.Annotations;
using TerrLauncherPackCreator.Code.Models;

namespace TerrLauncherPackCreator.Code.ViewModels
{
    public class AuthorEditorWindowViewModel
    {
        [NotNull]
        public AuthorItemModel AuthorModel { get; }

        public AuthorEditorWindowViewModel([NotNull] AuthorItemModel authorModel)
        {
            AuthorModel = authorModel;
        }
    }
}
