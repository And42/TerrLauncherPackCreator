using System.Collections.ObjectModel;
using CommonLibrary.CommonUtils;
using CrossPlatform.Code.Models;
using MVVM_Tools.Code.Commands;
using TerrLauncherPackCreator.Code.Models;

namespace TerrLauncherPackCreator.Pages.PackCreation;

public partial class PackCreationViewModel
{
    public ObservableCollection<AuthorItemModel> Authors { get; } = new();
    
    public IActionCommand AddAuthorCommand { get; private init; } = null!;
    public IActionCommand<AuthorItemModel> DeleteAuthorCommand { get; private init; } = null!;

    private object? InitializeStep4
    {
        // ReSharper disable once ValueParameterNotUsed
        init
        {
            AddAuthorCommand = new ActionCommand(AddAuthorCommand_Execute);
            DeleteAuthorCommand = new ActionCommand<AuthorItemModel>(DeleteAuthorCommand_Execute);
        }
    }
    
    private void InitStep4FromPackModel(PackModel packModel)
    {
        Authors.Clear();
        
        foreach (var author in packModel.Authors)
        {
            Authors.Add(
                new AuthorItemModel(
                    name: author.Name,
                    color: author.Color?.ToMediaColor(),
                    image: author.Icon,
                    link: author.Link,
                    iconHeight: author.IconHeight
                )
            );
        }
    }

    private void AddAuthorCommand_Execute()
    {
        Authors.Add(new AuthorItemModel());
    }

    private void DeleteAuthorCommand_Execute(AuthorItemModel author)
    {
        Authors.Remove(author);
    }
}