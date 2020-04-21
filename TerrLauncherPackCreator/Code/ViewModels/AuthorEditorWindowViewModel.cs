using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using CommonLibrary.CommonUtils;
using JetBrains.Annotations;
using MVVM_Tools.Code.Commands;
using TerrLauncherPackCreator.Code.Implementations;
using TerrLauncherPackCreator.Code.Json;
using TerrLauncherPackCreator.Code.Models;
using TerrLauncherPackCreator.Code.Utils;

namespace TerrLauncherPackCreator.Code.ViewModels
{
    public class AuthorEditorWindowViewModel : ViewModelBase
    {
        [NotNull]
        private readonly AuthorsFileProcessor _authorsFileProcessor;

        [NotNull]
        public AuthorItemModel EditableAuthor { get; }
        
        [NotNull]
        public IActionCommand<string> DropAuthorImageCommand { get; }
        
        [NotNull]
        public IActionCommand<AuthorJson> DeleteSavedAuthor { get; }
        
        [NotNull]
        public IActionCommand SaveAuthor { get; }
        
        [NotNull]
        public ObservableCollection<AuthorJson> SavedAuthors { get; }

        [CanBeNull]
        public AuthorJson SelectedSavedAuthor
        {
            get => _selectedSavedAuthor;
            set => SetProperty(ref _selectedSavedAuthor, value);
        }
        [CanBeNull]
        private AuthorJson _selectedSavedAuthor;

        public AuthorEditorWindowViewModel(
            [NotNull] AuthorItemModel editableAuthor,
            [NotNull] AuthorsFileProcessor authorsFileProcessor
        )
        {
            _authorsFileProcessor = authorsFileProcessor;
            EditableAuthor = editableAuthor;
            SavedAuthors = new ObservableCollection<AuthorJson>();
            DropAuthorImageCommand = new ActionCommand<string>(DropAuthorImage_Execute, DropAuthorImageCommand_CanExecute);
            DeleteSavedAuthor = new ActionCommand<AuthorJson>(DeleteSavedAuthor_Execute);
            SaveAuthor = new ActionCommand(SaveAuthor_Execute);

            if (File.Exists(Paths.AuthorsFile))
            {
                AuthorsJson savedAuthors = _authorsFileProcessor.ModelFromFile(Paths.AuthorsFile);
                savedAuthors.Authors?.Select(it => new AuthorJson
                {
                    Name = it.Name,
                    Color = it.Color,
                    Link = it.Link,
                    Icon = it.Icon
                }).ForEach(SavedAuthors.Add);
            }
            
            PropertyChanged += OnPropertyChanged;
        }

        private void SaveAuthor_Execute()
        {
            SavedAuthors.Add(new AuthorJson
            {
                Name = EditableAuthor.Name,
                Color = EditableAuthor.Color,
                Icon = EditableAuthor.Image == null
                    ? null
                    : new AuthorJson.IconJson {
                        Bytes = EditableAuthor.Image.Bytes,
                        Type = EditableAuthor.Image.Type
                    },
                Link = EditableAuthor.Link
            });
            WriteAuthorsToFile();
        }

        private void DropAuthorImage_Execute([NotNull] string iconPath) {
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

        private void DeleteSavedAuthor_Execute([CanBeNull] AuthorJson obj)
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
            _authorsFileProcessor.ModelToFile(model, Paths.AuthorsFile);
        }
        
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(SelectedSavedAuthor):
                    if (SelectedSavedAuthor == null)
                        return;

                    EditableAuthor.Name = SelectedSavedAuthor.Name;
                    EditableAuthor.Color = SelectedSavedAuthor.Color;
                    EditableAuthor.Link = SelectedSavedAuthor.Link;
                    if (SelectedSavedAuthor.Icon != null) {
                        EditableAuthor.Image = new ImageInfo(
                            SelectedSavedAuthor.Icon.Bytes, SelectedSavedAuthor.Icon.Type
                        );
                    }

                    break;
            }
        }
    }
}
