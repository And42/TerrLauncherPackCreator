using System.Windows;
using MVVM_Tools.Code.Commands;
using TerrLauncherPackCreator.Code.Utils;

namespace TerrLauncherPackCreator.Presentation.Controls;

public partial class DragAndDropImage
{
    public static readonly DependencyProperty DropImageCommandProperty = DependencyProperty.Register(
        nameof(DropImageCommand), typeof(IActionCommand<string>), typeof(DragAndDropImage), new PropertyMetadata(default(IActionCommand<string>)));

    public IActionCommand<string> DropImageCommand
    {
        get => (IActionCommand<string>) GetValue(DropImageCommandProperty);
        set => SetValue(DropImageCommandProperty, value);
    }

    public static readonly DependencyProperty ImagePathProperty = DependencyProperty.Register(
        nameof(ImagePath), typeof(string), typeof(DragAndDropImage),
        new FrameworkPropertyMetadata(default(string))
        {
            BindsTwoWayByDefault = true
        }
    );

    public string ImagePath
    {
        get => (string) GetValue(ImagePathProperty);
        set => SetValue(ImagePathProperty, value);
    }

    public static readonly DependencyProperty ImageHelpTextProperty = DependencyProperty.Register(
        nameof(ImageHelpText), typeof(string), typeof(DragAndDropImage),
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
        
    public DragAndDropImage()
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
                ImagePath = files[0];
                DropImageCommand.Execute(files[0]);
            }
        );
    }
}