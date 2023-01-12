using System;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Windows.Media;
using CommonLibrary.CommonUtils;
using CrossPlatform.Code.Utils;
using Microsoft.Win32;
using MVVM_Tools.Code.Commands;
using TerrLauncherPackCreator.Code.Interfaces;
using TerrLauncherPackCreator.Code.Json;
using TerrLauncherPackCreator.Code.Utils;
using TerrLauncherPackCreator.Resources.Localizations;
using TerrLauncherPackCreator.Windows;

namespace TerrLauncherPackCreator.Code.ViewModels
{
    public class PackStartupWindowViewModel : ViewModelBase
    {
        private readonly IAttachedWindowManipulator _attachedWindowManipulator;
        private readonly AppSettingsJson _appSettings;

        public string CurrentLanguage => Thread.CurrentThread.CurrentUICulture.Name;
        public bool EnglishLanguageActive => CurrentLanguage == "en-US";
        public bool RussianLanguageActive => CurrentLanguage == "ru-RU";
        public SolidColorBrush WindowBackground { get; } = new(
            // ReSharper disable once PossibleNullReferenceException
            // ReSharper disable once UnreachableCode
            // todo: check from settings
            (Color) ColorConverter.ConvertFromString(true ? "#ef5350" : "#66bb6a")
        );
        
        public Action? RecreateWindow { get; init; }

        public IActionCommand CreateNewPackCommand { get; }
        public IActionCommand ChooseExistingPackCommand { get; }
        public IActionCommand LaunchConverterCommand { get; }
        public IActionCommand SwitchToEnglishCommand { get; }
        public IActionCommand SwitchToRussianCommand { get; }

        public PackStartupWindowViewModel(
            IAttachedWindowManipulator attachedWindowManipulator,
            AppSettingsJson appSettings
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
        
        private void SwitchLanguageTo(string language)
        {
            if (Thread.CurrentThread.CurrentUICulture.Name == language)
                return;

            Thread.CurrentThread.CurrentUICulture = new CultureInfo(language);
            string oldLanguage = _appSettings.AppLanguage;
            _appSettings.AppLanguage = language;
            try
            {
                AppUtils.SaveAppSettings(_appSettings);
            }
            catch (Exception ex)
            {
                _appSettings.AppLanguage = oldLanguage;
                MessageBoxUtils.ShowError($"{StringResources.CantSaveAppSettings} {ex.Message}");
            }
            RecreateWindow?.Invoke();
        }
        
        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
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
