using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using CommonLibrary.CommonUtils;
using JetBrains.Annotations;
using MVVM_Tools.Code.Commands;
using Newtonsoft.Json;
using TerrLauncherPackCreator.Code.Json;
using TerrLauncherPackCreator.Code.Models;
using TerrLauncherPackCreator.Code.Utils;

namespace TerrLauncherPackCreator.Code.ViewModels
{
    public class AuthorEditorWindowViewModel : ViewModelBase
    {
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

        public AuthorEditorWindowViewModel([NotNull] AuthorItemModel editableAuthor)
        {
            EditableAuthor = editableAuthor;
            SavedAuthors = new ObservableCollection<AuthorJson>();
            DropAuthorImageCommand = new ActionCommand<string>(DropAuthorImage_Execute, DropAuthorImageCommand_CanExecute);
            DeleteSavedAuthor = new ActionCommand<AuthorJson>(DeleteSavedAuthor_Execute);
            SaveAuthor = new ActionCommand(SaveAuthor_Execute);

            if (File.Exists(Paths.AuthorsFile))
            {
                var savedAuthors = JsonConvert.DeserializeObject<AuthorsJson>(File.ReadAllText(Paths.AuthorsFile));
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
                Icon = EditableAuthor.ImageBytes,
                Link = EditableAuthor.Link
            });
            WriteAuthorsToFile();
        }

        private void DropAuthorImage_Execute([NotNull] string iconPath)
        {
            EditableAuthor.ImageBytes = File.ReadAllBytes(iconPath);
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
            return !Working && File.Exists(filePath) && Path.GetExtension(filePath) == ".png";
        }

        private void WriteAuthorsToFile()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(Paths.AuthorsFile));
            File.WriteAllText(Paths.AuthorsFile, JsonConvert.SerializeObject(
                new AuthorsJson
                {
                    Authors = SavedAuthors.ToList()
                }, Formatting.Indented
            ));
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
                    if (SelectedSavedAuthor.Icon != null)
                        EditableAuthor.ImageBytes = SelectedSavedAuthor.Icon;
                    break;
            }
        }
    }
}
