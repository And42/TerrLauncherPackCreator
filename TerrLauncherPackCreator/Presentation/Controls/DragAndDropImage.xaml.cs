using System.Windows;
using System.Windows.Input;
using MVVM_Tools.Code.Commands;
using TerrLauncherPackCreator.Code.Utils;

namespace TerrLauncherPackCreator.Presentation.Controls;

public partial class DragAndDropImage
{
    public static readonly DependencyProperty DropImageCommandProperty = DependencyProperty.Register(
        nameof(DropImageCommand), typeof(IActionCommand<string>), typeof(DragAndDropImage), new PropertyMetadata(default(IActionCommand<string>)));

    public IActionCommand<string>? DropImageCommand
    {
        get => (IActionCommand<string>) GetValue(DropImageCommandProperty);
        set => SetValue(DropImageCommandProperty, value);
    }
    
    public static readonly DependencyProperty ImageClickCommandProperty = DependencyProperty.Register(
        nameof(ImageClickCommand), typeof(IActionCommand), typeof(DragAndDropImage), new PropertyMetadata(default(IActionCommand)));

    public IActionCommand? ImageClickCommand
    {
        get => (IActionCommand) GetValue(ImageClickCommandProperty);
        set => SetValue(ImageClickCommandProperty, value);
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
        var command = DropImageCommand;
        if (command == null)
            return;
        
        DragDropUtils.HandleDrag(
            e, 
            files => files.Length == 1 && command.CanExecute(files[0]),
            DragDropEffects.Copy
        );
    }

    private void Icon_OnDrop(object sender, DragEventArgs e)
    {
        var command = DropImageCommand;
        if (command == null)
            return;
        
        DragDropUtils.HandleDrop(
            e,
            files => files.Length == 1,
            files =>
            {
                ImagePath = files[0];
                command.Execute(files[0]);
            }
        );
    }

    private void Icon_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        var command = ImageClickCommand;
        if (command == null)
            return;
        
        if (command.CanExecute(null))
            command.Execute();
    }
}