using JetBrains.Annotations;
using TerrLauncherPackCreator.Code.Implementations;
using TerrLauncherPackCreator.Code.Models;
using TerrLauncherPackCreator.Code.ViewModels;

namespace TerrLauncherPackCreator.Windows
{
    public partial class AuthorEditorWindow
    {
        public AuthorEditorWindow(
            [NotNull] AuthorItemModel authorModel,
            [NotNull] AuthorsFileProcessor authorsFileProcessor
        )
        {
            InitializeComponent();

            ViewModel = new AuthorEditorWindowViewModel(authorModel, authorsFileProcessor);
        }

        public AuthorEditorWindowViewModel ViewModel
        {
            get => DataContext as AuthorEditorWindowViewModel;
            set => DataContext = value;
        }
    }
}
