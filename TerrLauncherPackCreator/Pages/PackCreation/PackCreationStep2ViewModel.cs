using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using CrossPlatform.Code.Models;
using CrossPlatform.Code.Utils;
using MVVM_Tools.Code.Commands;
using TerrLauncherPackCreator.Code.Models;

namespace TerrLauncherPackCreator.Pages.PackCreation;

public partial class PackCreationViewModel
{
    private static readonly IReadOnlySet<string> PreviewExtensions = new HashSet<string> {".jpg", ".png", ".gif"};

    public ObservableCollection<PreviewItemModel> Previews { get; } = new();
    
    public IActionCommand<string[]> DropPreviewsCommand { get; private init; } = null!;
    public IActionCommand<PreviewItemModel> DeletePreviewItemCommand { get; private init; } = null!;

    private object? InitializeStep2
    {
        // ReSharper disable once ValueParameterNotUsed
        init
        {
            DropPreviewsCommand =
                new ActionCommand<string[]>(DropPreviewsCommand_Execute, DropPreviewsCommand_CanExecute);
            DeletePreviewItemCommand = new ActionCommand<PreviewItemModel>(DeletePreviewItemCommand_Execute,
                DeletePreviewItemCommand_CanExecute);
            
            ResetStep2Collections();
            
            PropertyChanged += OnPropertyChangedStep2;
        }
    }

    private void InitStep2FromPackModel(PackModel packModel)
    {
        ResetStep2Collections();
        
        var previewItems = packModel.PreviewsPaths.Select(PreviewItemModel.FromImageFile);
        previewItems.ForEach(Previews.Add);
    }

    private void ResetStep2Collections()
    {
        Previews.Clear();
        
        Previews.Add(new PreviewItemModel(filePath: null, isDragDropTarget: true));
    }
    
    private bool DropPreviewsCommand_CanExecute(string[] files)
    {
        return !Working && files != null &&
               files.All(it => File.Exists(it) && PreviewExtensions.Contains(Path.GetExtension(it)));
    }

    private void DropPreviewsCommand_Execute(string[] files)
    {
        foreach (string file in files)
        {
            if (Previews.Any(item => item.FilePath == file))
                continue;

            Previews.Add(new PreviewItemModel(file, false));
        }
    }

    private bool DeletePreviewItemCommand_CanExecute(PreviewItemModel previewItem)
    {
        return !Working && !previewItem.IsDragDropTarget;
    }

    private void DeletePreviewItemCommand_Execute(PreviewItemModel previewItem)
    {
        Previews.Remove(previewItem);
    }
    
    private void OnPropertyChangedStep2(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(Working):
                DropPreviewsCommand.RaiseCanExecuteChanged();
                DeletePreviewItemCommand.RaiseCanExecuteChanged();
                break;
        }
    }
}