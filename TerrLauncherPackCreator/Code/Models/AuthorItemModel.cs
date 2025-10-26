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
        get;
        set => SetProperty(ref field, value);
    }

    public string Name
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Color? Color
    {
        get;
        set => SetProperty(ref field, value);
    }

    public string Link
    {
        get;
        set => SetProperty(ref field, value);
    }

    public int IconHeight
    {
        get;
        set => SetProperty(ref field, value);
    }

    public IActionCommand<AuthorItemModel> EditAuthorCommand { get; }

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
        Name = name;
        Color = color;
        Image = image;
        Link = link;
        IconHeight = iconHeight;
            
        EditAuthorCommand = new ActionCommand<AuthorItemModel>(EditAuthorCommand_Execute);
    }

    private static void EditAuthorCommand_Execute(AuthorItemModel authorModel)
    {
        new AuthorEditorWindow(authorModel).ShowDialog();
    }
}