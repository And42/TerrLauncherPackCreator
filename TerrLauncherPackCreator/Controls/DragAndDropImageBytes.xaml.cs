﻿using System.IO;
using System.Windows;
using MVVM_Tools.Code.Commands;
using TerrLauncherPackCreator.Code.Utils;

namespace TerrLauncherPackCreator.Controls
{
    public partial class DragAndDropImageBytes
    {
        public static readonly DependencyProperty DropImageCommandProperty = DependencyProperty.Register(
            nameof(DropImageCommand), typeof(IActionCommand<string>),
            typeof(DragAndDropImageBytes), new PropertyMetadata(default(IActionCommand<string>)));

        public IActionCommand<string> DropImageCommand
        {
            get => (IActionCommand<string>) GetValue(DropImageCommandProperty);
            set => SetValue(DropImageCommandProperty, value);
        }

        public static readonly DependencyProperty ImageBytesProperty = DependencyProperty.Register(
            nameof(ImageBytes), typeof(byte[]), typeof(DragAndDropImageBytes),
            new FrameworkPropertyMetadata(default(byte[]))
            {
                BindsTwoWayByDefault = true
            }
        );

        public byte[] ImageBytes
        {
            get => (byte[]) GetValue(ImageBytesProperty);
            set => SetValue(ImageBytesProperty, value);
        }

        public static readonly DependencyProperty ImageHelpTextProperty = DependencyProperty.Register(
            nameof(ImageHelpText), typeof(string), typeof(DragAndDropImageBytes),
            new FrameworkPropertyMetadata(default(string))
            {
                BindsTwoWayByDefault = true
            }
        );

        public string ImageHelpText
        {
            get => (string) GetValue(ImageHelpTextProperty);
            set => SetValue(ImageHelpTextProperty, value);
        }
        
        public DragAndDropImageBytes()
        {
            InitializeComponent();
        }
        
        private void Icon_OnDragOver(object sender, DragEventArgs e)
        {
            DragDropUtils.HandleDrag(
                e, 
                files => files.Length == 1 && DropImageCommand.CanExecute(files[0]),
                DragDropEffects.Copy
            );
        }

        private void Icon_OnDrop(object sender, DragEventArgs e)
        {
            DragDropUtils.HandleDrop(
                e,
                files => files.Length == 1,
                files =>
                {
                    ImageBytes = File.ReadAllBytes(files[0]);
                    DropImageCommand.Execute(files[0]);
                }
            );
        }
    }
}