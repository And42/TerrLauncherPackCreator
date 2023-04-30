using TerrLauncherPackCreator.Code.Models;
using TerrLauncherPackCreator.Code.ViewModels;

namespace TerrLauncherPackCreator.Windows;

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