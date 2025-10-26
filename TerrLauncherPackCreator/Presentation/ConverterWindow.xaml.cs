using System;
using System.Windows;
using TerrLauncherPackCreator.Code.Utils;

namespace TerrLauncherPackCreator.Presentation;

public partial class ConverterWindow
{
    public ConverterWindow()
    {
        InitializeComponent();
            
        ViewModel = new ConverterWindowViewModel();
    }

    private ConverterWindowViewModel ViewModel
    {
        get => DataContext as ConverterWindowViewModel ?? throw new InvalidOperationException();
        init => DataContext = value;
    }

    private void SourceFiles_OnDragOver(object sender, DragEventArgs e)
    {
        DragDropUtils.HandleDrag(e, ViewModel.DropSourceFilesCommand.CanExecute, DragDropEffects.Copy);
    }

    private void SourceFiles_OnDrop(object sender, DragEventArgs e)
    {
        DragDropUtils.HandleDrop(e, ViewModel.DropSourceFilesCommand.CanExecute, ViewModel.DropSourceFilesCommand.Execute);
    }
        
    private void ConvertedFiles_OnDragOver(object sender, DragEventArgs e)
    {
        DragDropUtils.HandleDrag(e, ViewModel.DropConvertedFiledCommand.CanExecute, DragDropEffects.Copy);
    }

    private void ConvertedFiles_OnDrop(object sender, DragEventArgs e)
    {
        DragDropUtils.HandleDrop(e, ViewModel.DropConvertedFiledCommand.CanExecute, ViewModel.DropConvertedFiledCommand.Execute);
    }
}