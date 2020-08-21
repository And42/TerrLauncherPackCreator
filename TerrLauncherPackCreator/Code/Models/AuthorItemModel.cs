using System.Windows.Media;
using JetBrains.Annotations;
using MVVM_Tools.Code.Commands;
using TerrLauncherPackCreator.Code.Implementations;
using TerrLauncherPackCreator.Code.Utils;
using TerrLauncherPackCreator.Code.ViewModels;
using TerrLauncherPackCreator.Windows;

namespace TerrLauncherPackCreator.Code.Models
{
    public class AuthorItemModel : ViewModelBase
    {
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

        public int IconHeight
        {
            get => _iconHeight;
            set => SetProperty(ref _iconHeight, value);
        }

        public IActionCommand<AuthorItemModel> EditAuthorCommand { get; }

        [CanBeNull] private ImageInfo _image;
        [CanBeNull] private string _name;
        [CanBeNull] private Color? _color;
        [CanBeNull] private string _link;
        private int _iconHeight;

        public AuthorItemModel(): this(
            name: null,
            color: null,
            image: null,
            link: null,
            iconHeight: PackUtils.DefaultAuthorIconHeight
        ) {}
        
        public AuthorItemModel(
            [CanBeNull] string name,
            [CanBeNull] Color? color,
            [CanBeNull] ImageInfo image,
            [CanBeNull] string link,
            int iconHeight
        )
        {
            _name = name;
            _color = color;
            _image = image;
            _link = link;
            _iconHeight = iconHeight;
            
            EditAuthorCommand = new ActionCommand<AuthorItemModel>(EditAuthorCommand_Execute);
        }

        private void EditAuthorCommand_Execute(AuthorItemModel authorModel)
        {
            new AuthorEditorWindow(authorModel).ShowDialog();
        }
    }
}
