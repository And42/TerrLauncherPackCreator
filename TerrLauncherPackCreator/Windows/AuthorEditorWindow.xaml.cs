using System.Windows;
using JetBrains.Annotations;
using TerrLauncherPackCreator.Code.Models;
using TerrLauncherPackCreator.Code.ViewModels;

namespace TerrLauncherPackCreator.Windows
{
    public partial class AuthorEditorWindow : Window
    {
        public AuthorEditorWindow([NotNull] AuthorItemModel authorModel)
        {
            InitializeComponent();

            ViewModel = new AuthorEditorWindowViewModel(authorModel);
        }

        public AuthorEditorWindowViewModel ViewModel
        {
            get => DataContext as AuthorEditorWindowViewModel;
            set => DataContext = value;
        }
    }
}
