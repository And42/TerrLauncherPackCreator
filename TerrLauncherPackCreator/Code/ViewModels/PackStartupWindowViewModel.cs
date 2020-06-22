using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Threading;
using CommonLibrary.CommonUtils;
using JetBrains.Annotations;
using Microsoft.Win32;
using MVVM_Tools.Code.Commands;
using Newtonsoft.Json;
using TerrLauncherPackCreator.Code.Interfaces;
using TerrLauncherPackCreator.Code.Json;
using TerrLauncherPackCreator.Code.Utils;
using TerrLauncherPackCreator.Resources.Localizations;
using TerrLauncherPackCreator.Windows;

namespace TerrLauncherPackCreator.Code.ViewModels
{
    public class PackStartupWindowViewModel : ViewModelBase
    {
        [NotNull]
        private readonly IAttachedWindowManipulator _attachedWindowManipulator;
        [NotNull]
        private readonly AppSettingsJson _appSettings;

        [NotNull]
        public string CurrentLanguage => Thread.CurrentThread.CurrentUICulture.Name;
        public bool EnglishLanguageActive => CurrentLanguage == "en-US";
        public bool RussianLanguageActive => CurrentLanguage == "ru-RU";
        
        [CanBeNull]
        public Action RecreateWindow { get; set; }

        [NotNull]
        public IActionCommand CreateNewPackCommand { get; }
        [NotNull]
        public IActionCommand ChooseExistingPackCommand { get; }
        [NotNull]
        public IActionCommand LaunchConverterCommand { get; }
        [NotNull]
        public IActionCommand SwitchToEnglishCommand { get; }
        [NotNull]
        public IActionCommand SwitchToRussianCommand { get; }

        public PackStartupWindowViewModel(
            [NotNull] IAttachedWindowManipulator attachedWindowManipulator,
            [NotNull] AppSettingsJson appSettings
        )
        {
            _attachedWindowManipulator = attachedWindowManipulator;
            _appSettings = appSettings;

            CreateNewPackCommand = new ActionCommand(CreateNewPackCommand_Execute);
            ChooseExistingPackCommand = new ActionCommand(ChooseExistingPackCommand_Execute);
            LaunchConverterCommand = new ActionCommand(LaunchConverterCommand_Execute);
            SwitchToEnglishCommand = new ActionCommand(() => SwitchLanguageTo("en-US"));
            SwitchToRussianCommand = new ActionCommand(() => SwitchLanguageTo("ru-RU"));
            
            PropertyChanged += OnPropertyChanged;
        }

        private void CreateNewPackCommand_Execute()
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();

            _attachedWindowManipulator.Close();
        }

        private void ChooseExistingPackCommand_Execute()
        {
            string filters = $"{StringResources.ChoosePackDialogFilter}|*{PackUtils.PacksExtension};*{PackUtils.PacksActualExtension}";

            var dialog = new OpenFileDialog
            {
                Title = StringResources.ChoosePackDialogTitle,
                Filter = filters,
                CheckFileExists = true
            };

            if (dialog.ShowDialog() != true)
            {
                MessageBoxUtils.ShowError(StringResources.ChoosePackDialogFailed);
                return;
            }
    
            var mainWindow = new MainWindow();
            mainWindow.Show();
            mainWindow.ViewModel.PackProcessor.LoadPackFromFile(dialog.FileName);

            _attachedWindowManipulator.Close();
        }

        private void LaunchConverterCommand_Execute()
        {
            new ConverterWindow().Show();
            _attachedWindowManipulator.Close();
        }
        
        private void SwitchLanguageTo([NotNull] string language)
        {
            if (Thread.CurrentThread.CurrentUICulture.Name == language)
                return;

            Thread.CurrentThread.CurrentUICulture = new CultureInfo(language);
            string oldLanguage = _appSettings.AppLanguage;
            _appSettings.AppLanguage = language;
            try
            {
                File.WriteAllText(Paths.AppSettingsFile, JsonConvert.SerializeObject(_appSettings));
            }
            catch (Exception ex)
            {
                _appSettings.AppLanguage = oldLanguage;
                MessageBoxUtils.ShowError($"{StringResources.CantSaveAppSettings} {ex.Message}");
            }
            RecreateWindow?.Invoke();
        }
        
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(CurrentLanguage):
                    OnPropertyChanged(nameof(EnglishLanguageActive));
                    OnPropertyChanged(nameof(RussianLanguageActive));
                    break;
            }
        }
    }
}
