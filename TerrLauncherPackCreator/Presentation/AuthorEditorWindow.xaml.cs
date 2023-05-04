using TerrLauncherPackCreator.Code.Models;

namespace TerrLauncherPackCreator.Presentation;

public partial class AuthorEditorWindow
{
    public AuthorEditorWindow(
        AuthorItemModel authorModel
    )
    {
        InitializeComponent();

        ViewModel = new AuthorEditorWindowViewModel(authorModel);
    }

    public AuthorEditorWindowViewModel ViewModel
    {
        init => DataContext = value;
    }
}