using System.Windows.Media;
using JetBrains.Annotations;
using MVVM_Tools.Code.Commands;
using TerrLauncherPackCreator.Code.Implementations;
using TerrLauncherPackCreator.Code.ViewModels;
using TerrLauncherPackCreator.Windows;

namespace TerrLauncherPackCreator.Code.Models
{
    public class AuthorItemModel : ViewModelBase
    {
        [NotNull]
        private readonly AuthorsFileProcessor _authorsFileProcessor;

        [CanBeNull]
        public ImageInfo Image
        {
            get => _image;
            set => SetProperty(ref _image, value);
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

        [CanBeNull] private ImageInfo _image;
        [CanBeNull] private string _name;
        [CanBeNull] private Color? _color;
        [CanBeNull] private string _link;

        public AuthorItemModel([NotNull] AuthorsFileProcessor authorsFileProcessor) {
            _authorsFileProcessor = authorsFileProcessor;
            EditAuthorCommand = new ActionCommand<AuthorItemModel>(EditAuthorCommand_Execute);
        }

        private void EditAuthorCommand_Execute(AuthorItemModel authorModel)
        {
            new AuthorEditorWindow(authorModel, _authorsFileProcessor).ShowDialog();
        }
    }
}
