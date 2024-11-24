using System;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using CommonLibrary.CommonUtils;
using CrossPlatform.Code.Utils;
using Microsoft.Win32;
using MVVM_Tools.Code.Commands;
using TerrLauncherPackCreator.Code.Interfaces;
using TerrLauncherPackCreator.Code.Json;
using TerrLauncherPackCreator.Code.Utils;
using TerrLauncherPackCreator.Code.ViewModels;
using TerrLauncherPackCreator.Resources.Localizations;

namespace TerrLauncherPackCreator.Presentation;

public class PackStartupWindowViewModel : ViewModelBase
{
    private readonly IAttachedWindowManipulator _attachedWindowManipulator;
    private readonly AppSettingsJson _appSettings;

    public string CurrentLanguage => Thread.CurrentThread.CurrentUICulture.Name;
    public bool EnglishLanguageActive => CurrentLanguage == "en-US";
    public bool RussianLanguageActive => CurrentLanguage == "ru-RU";
    public bool PackStructureVersion26Active => IsPackStructureVersionActive(PackUtils.PackStructureVersions.V26);
    public bool PackStructureVersion27Active => IsPackStructureVersionActive(PackUtils.PackStructureVersions.V27);

    public Action? RecreateWindow { get; init; }

    public IActionCommand CreateNewPackCommand { get; }
    public IActionCommand ChooseExistingPackCommand { get; }
    public IActionCommand LaunchConverterCommand { get; }
    public IActionCommand SwitchToEnglishCommand { get; }
    public IActionCommand SwitchToRussianCommand { get; }
    public IActionCommand<int> ChangeStructureVersion { get; }

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
        ChangeStructureVersion = new ActionCommand<int>(ChangeStructureVersion_Execute);
            
        PropertyChanged += OnPropertyChanged;
            
        ValidatePackStructureVersion();
    }
        
    private bool IsPackStructureVersionActive(int version)
    {
        return _appSettings.PackStructureVersion == version;
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
        
    private void ChangeStructureVersion_Execute(int version)
    {
        int oldVersion = _appSettings.PackStructureVersion;
        if (oldVersion == version)
            return;

        _appSettings.PackStructureVersion = version;
        try
        {
            AppUtils.SaveAppSettings(_appSettings);
        }
        catch (Exception ex)
        {
            _appSettings.PackStructureVersion = oldVersion;
            MessageBoxUtils.ShowError($"{StringResources.CantSaveAppSettings} {ex.Message}");
        }
        OnPropertyChanged(nameof(PackStructureVersion26Active));
        OnPropertyChanged(nameof(PackStructureVersion27Active));
            
        (1 / (27 / PackUtils.PackStructureVersions.Latest)).Ignore();
    }

    private void ValidatePackStructureVersion()
    {
        if (_appSettings.PackStructureVersion < PackUtils.PackStructureVersions.Oldest)
            ChangeStructureVersion.Execute(PackUtils.PackStructureVersions.Oldest);
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