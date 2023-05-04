using System.Windows.Media;
using CrossPlatform.Code.Implementations;
using CrossPlatform.Code.Utils;
using MVVM_Tools.Code.Commands;
using TerrLauncherPackCreator.Code.ViewModels;
using TerrLauncherPackCreator.Presentation;

namespace TerrLauncherPackCreator.Code.Models;

public class AuthorItemModel : ViewModelBase
{
    public ImageInfo? Image
    {
        get => _image;
        set => SetProperty(ref _image, value);
    }

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public Color? Color
    {
        get => _color;
        set => SetProperty(ref _color, value);
    }

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

    private ImageInfo? _image;
    private string _name;
    private Color? _color;
    private string _link;
    private int _iconHeight;

    public AuthorItemModel(): this(
        name: string.Empty,
        color: null,
        image: null,
        link: string.Empty,
        iconHeight: PackUtils.DefaultAuthorIconHeight
    ) {}
        
    public AuthorItemModel(
        string name,
        Color? color,
        ImageInfo? image,
        string link,
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