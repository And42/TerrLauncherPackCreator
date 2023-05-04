using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using CrossPlatform.Code.Implementations;
using CrossPlatform.Code.Utils;
using MVVM_Tools.Code.Commands;
using TerrLauncherPackCreator.Code.Json;
using TerrLauncherPackCreator.Code.Models;
using TerrLauncherPackCreator.Code.Utils;
using TerrLauncherPackCreator.Code.ViewModels;

namespace TerrLauncherPackCreator.Windows;

public class AuthorEditorWindowViewModel : ViewModelBase
{
    public AuthorItemModel EditableAuthor { get; }
        
    public IActionCommand<string> DropAuthorImageCommand { get; }
        
    public IActionCommand<AuthorJson> DeleteSavedAuthor { get; }
        
    public IActionCommand SaveAuthor { get; }
        
    public ObservableCollection<AuthorJson> SavedAuthors { get; }

    public AuthorJson? SelectedSavedAuthor
    {
        get => _selectedSavedAuthor;
        set => SetProperty(ref _selectedSavedAuthor, value);
    }
    private AuthorJson? _selectedSavedAuthor;

    public AuthorEditorWindowViewModel(
        AuthorItemModel editableAuthor
    )
    {
        EditableAuthor = editableAuthor;
        SavedAuthors = new ObservableCollection<AuthorJson>();
        DropAuthorImageCommand = new ActionCommand<string>(DropAuthorImage_Execute, DropAuthorImageCommand_CanExecute);
        DeleteSavedAuthor = new ActionCommand<AuthorJson>(DeleteSavedAuthor_Execute);
        SaveAuthor = new ActionCommand(SaveAuthor_Execute);

        if (File.Exists(Paths.AuthorsFile))
        {
            AuthorsJson savedAuthors = AuthorsJson.Processor.Deserialize(File.ReadAllText(Paths.AuthorsFile));
            savedAuthors.Authors?.Select(it => new AuthorJson(
                name: it.Name,
                color: it.Color,
                link: it.Link,
                icon: it.Icon,
                iconHeight: it.IconHeight
            )).ForEach(SavedAuthors.Add);
        }
            
        PropertyChanged += OnPropertyChanged;
    }

    private void SaveAuthor_Execute()
    {
        SavedAuthors.Add(new AuthorJson(
            name: EditableAuthor.Name,
            color: EditableAuthor.Color,
            icon: EditableAuthor.Image == null
                ? null
                : new AuthorJson.IconJson {
                    Bytes = EditableAuthor.Image.Bytes,
                    Type = EditableAuthor.Image.Type
                },
            link: EditableAuthor.Link,
            iconHeight: EditableAuthor.IconHeight
        ));
        WriteAuthorsToFile();
    }

    private void DropAuthorImage_Execute(string iconPath) {
        ImageInfo.ImageType imageType;
        switch (Path.GetExtension(iconPath)) {
            case ".png":
                imageType = ImageInfo.ImageType.Png;
                break;
            case ".gif":
                imageType = ImageInfo.ImageType.Gif;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(iconPath), iconPath, @"Unknown extension");
        }
            
        EditableAuthor.Image = new ImageInfo(File.ReadAllBytes(iconPath), imageType);
    }

    private void DeleteSavedAuthor_Execute(AuthorJson? obj)
    {
        if (obj == null)
            return;

        SavedAuthors.Remove(obj);
        WriteAuthorsToFile();
    }

    private bool DropAuthorImageCommand_CanExecute(string filePath)
    {
        if (Working || !File.Exists(filePath)) {
            return false;
        }

        string extension = Path.GetExtension(filePath);
        return extension == ".png" || extension == ".gif";
    }

    private void WriteAuthorsToFile() {
        var model = AuthorsJson.CreateLatest();
        model.Authors = SavedAuthors.ToList();
        IOUtils.EnsureParentDirExists(Paths.AuthorsFile);
        File.WriteAllText(Paths.AuthorsFile, AuthorsJson.Processor.Serialize(model), Encoding.UTF8);
    }
        
    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(SelectedSavedAuthor):
                if (SelectedSavedAuthor == null)
                    return;

                EditableAuthor.Name = SelectedSavedAuthor.Name ?? string.Empty;
                EditableAuthor.Color = SelectedSavedAuthor.Color;
                EditableAuthor.Link = SelectedSavedAuthor.Link ?? string.Empty;
                EditableAuthor.IconHeight = SelectedSavedAuthor.IconHeight;
                if (SelectedSavedAuthor.Icon != null) {
                    EditableAuthor.Image = new ImageInfo(
                        SelectedSavedAuthor.Icon.Bytes, SelectedSavedAuthor.Icon.Type
                    );
                }

                break;
        }
    }
}