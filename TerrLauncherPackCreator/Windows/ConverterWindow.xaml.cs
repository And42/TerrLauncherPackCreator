﻿using System.Windows;
using TerrLauncherPackCreator.Code.Utils;
using TerrLauncherPackCreator.Code.ViewModels;

namespace TerrLauncherPackCreator.Windows
{
    public partial class ConverterWindow
    {
        public ConverterWindow()
        {
            InitializeComponent();
            
            ViewModel = new ConverterWindowViewModel();
        }

        public ConverterWindowViewModel ViewModel
        {
            get => DataContext as ConverterWindowViewModel;
            set => DataContext = value;
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
}