using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Threading;
using System.Windows.Controls;
using CommonLibrary.CommonUtils;
using CrossPlatform.Code.Implementations;
using CrossPlatform.Code.Interfaces;
using MVVM_Tools.Code.Commands;
using TerrLauncherPackCreator.Code.Implementations;
using TerrLauncherPackCreator.Code.Json;
using TerrLauncherPackCreator.Code.Utils;
using TerrLauncherPackCreator.Code.ViewModels;
using TerrLauncherPackCreator.Presentation.PackCreation;
using TerrLauncherPackCreator.Resources.Localizations;

namespace TerrLauncherPackCreator.Presentation;

public class MainWindowViewModel : ViewModelBase
{
    private class ProgressManager : ViewModelBase, IProgressManager
    {
        private const int UpdateTextDelayMs = 500;
        // ReSharper disable once NotAccessedField.Local
        private readonly Timer _updateTextTimer;

        private string[]? _steps;
        private int _step;
        private string _text = "";

        public string Text
        {
            get => _text;
            set
            {
                if (!SetProperty(ref _text, value))
                    return;

                _steps =
                [
                    value,
                    value + ".",
                    value + "..",
                    value + "..."
                ];
            }
        }

        public int CurrentProgress
        {
            get;
            set => SetProperty(ref field, value);
        }

        public int MaximumProgress
        {
            get;
            set => SetProperty(ref field, value);
        }

        public int RemainingFilesCount
        {
            get;
            set => SetProperty(ref field, value);
        }

        public bool IsIndeterminate
        {
            get;
            set => SetProperty(ref field, value);
        }

        public ProgressManager()
        {
            _updateTextTimer = new Timer(_ =>
            {
                if (_steps == null)
                    return;

                if (_step >= _steps.Length)
                    _step = 0;

                _text = _steps[_step];

                _step++;

                OnPropertyChanged(nameof(Text));
            }, null, 0, UpdateTextDelayMs);
        }
    }

    public string WindowTitle { get; }
    public int CurrentStep
    {
        get => _currentStep;
        set => SetProperty(ref _currentStep, value);
    }
    private int _currentStep;

    public double InitialWindowWidth { get; }
    public double InitialWindowHeight { get; }

    public Page[] StepsPages { get; }

    public ObservableCollection<IProgressManager> ProgressManagers { get; }
    public IPackProcessor PackProcessor { get; }
    public IActionCommand GoToPreviousStepCommand { get; }
    public IActionCommand GoToNextStepCommand { get; }

    public MainWindowViewModel(
        Action? restartApp
    )
    {
        WindowTitle = Assembly.GetEntryAssembly()?.GetName().Name ?? string.Empty;
        _currentStep = 1;
        InitialWindowWidth = ValuesProvider.AppSettings.MainWindowWidth;
        InitialWindowHeight = ValuesProvider.AppSettings.MainWindowHeight;

        GoToPreviousStepCommand = new ActionCommand(
            GoToPreviousStepCommand_Execute, 
            GoToPreviousStepCommand_CanExecute
        );
        GoToNextStepCommand = new ActionCommand(
            GoToNextStepCommand_Execute,
            GoToNextStepCommand_CanExecute
        );

        var loadProgressManager = new ProgressManager {Text = StringResources.LoadingProgressStep};
        var saveProgressManager = new ProgressManager {Text = StringResources.SavingProcessStep};
        var fileConverter = new FileConverter(SessionHelper.Instance);

        PackProcessor = new PackProcessor(
            loadProgressManager,
            saveProgressManager,
            fileConverter,
            SessionHelper.Instance,
            new ImageConverter()
        );
        var tempDirsProvider = new TempDirsProvider(Paths.TempDir);
        tempDirsProvider.DeleteAll();
            
        var packCreationViewModel = new PackCreationViewModel(PackProcessor, ValuesProvider.AppSettings, restartApp);

        StepsPages =
        [
            new PackCreationStep1(packCreationViewModel), 
            new PackCreationStep2(packCreationViewModel), 
            new PackCreationStep3(packCreationViewModel), 
            new PackCreationStep4(packCreationViewModel), 
            new PackCreationStep5(packCreationViewModel)
        ];

        ProgressManagers =
        [
            loadProgressManager,
            saveProgressManager
        ];

        PropertyChanged += OnPropertyChanged;
    }

    private bool GoToPreviousStepCommand_CanExecute()
    {
        return CurrentStep > 1;
    }

    private void GoToPreviousStepCommand_Execute()
    {
        CurrentStep--;
    }

    private bool GoToNextStepCommand_CanExecute()
    {
        return CurrentStep < StepsPages.Length;
    }

    private void GoToNextStepCommand_Execute()
    {
        CurrentStep++;
    }

    public void OnWindowClosed(int actualWidth, int actualHeight)
    {
        AppSettingsJson appSettings = ValuesProvider.AppSettings;
        appSettings.MainWindowWidth = actualWidth;
        appSettings.MainWindowHeight = actualHeight;
        try
        {
            AppUtils.SaveAppSettings(appSettings);
        }
        catch (Exception ex)
        {
            MessageBoxUtils.ShowError($"{StringResources.CantSaveAppSettings} {ex.Message}");
        }
    }
        
    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(CurrentStep):
                GoToPreviousStepCommand.RaiseCanExecuteChanged();
                GoToNextStepCommand.RaiseCanExecuteChanged();
                break;
        }
    }
}