using System.IO;
using JetBrains.Annotations;
using MVVM_Tools.Code.Commands;
using TerrLauncherPackCreator.Code.Models;

namespace TerrLauncherPackCreator.Code.ViewModels
{
    public class AuthorEditorWindowViewModel : ViewModelBase
    {
        [NotNull]
        public AuthorItemModel AuthorModel { get; }
        [NotNull]
        public IActionCommand<string> DropAuthorImageCommand { get; }

        public AuthorEditorWindowViewModel([NotNull] AuthorItemModel authorModel)
        {
            AuthorModel = authorModel;
            DropAuthorImageCommand = new ActionCommand<string>(file => {}, DropAuthorImageCommand_CanExecute);
        }
        
        private bool DropAuthorImageCommand_CanExecute(string filePath)
        {
            return !Working && File.Exists(filePath) && Path.GetExtension(filePath) == ".png";
        }
    }
}
