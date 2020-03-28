using System.IO;
using System.Windows.Media;
using JetBrains.Annotations;
using MVVM_Tools.Code.Commands;
using TerrLauncherPackCreator.Code.ViewModels;
using TerrLauncherPackCreator.Windows;

namespace TerrLauncherPackCreator.Code.Models
{
    public class AuthorItemModel : ViewModelBase
    {
        [CanBeNull]
        public byte[] ImageBytes
        {
            get => _imageBytes;
            set => SetProperty(ref _imageBytes, value);
        }

        [CanBeNull]
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        [CanBeNull]
        public Color? Color
        {
            get => _color;
            set => SetProperty(ref _color, value);
        }

        [CanBeNull]
        public string Link
        {
            get => _link;
            set => SetProperty(ref _link, value);
        }

        public IActionCommand<AuthorItemModel> EditAuthorCommand { get; }

        [CanBeNull] private byte[] _imageBytes;
        [CanBeNull] private string _name;
        [CanBeNull] private Color? _color;
        [CanBeNull] private string _link;

        public AuthorItemModel()
        {
            EditAuthorCommand = new ActionCommand<AuthorItemModel>(EditAuthorCommand_Execute);
        }

        private void EditAuthorCommand_Execute(AuthorItemModel authorModel)
        {
            new AuthorEditorWindow(authorModel).ShowDialog();
        }
    }
}
