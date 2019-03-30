using System.Collections.ObjectModel;
using System.Reflection;
using System.Threading;
using System.Windows.Controls;
using MVVM_Tools.Code.Classes;
using MVVM_Tools.Code.Commands;
using MVVM_Tools.Code.Providers;
using TerrLauncherPackCreator.Code.Implementations;
using TerrLauncherPackCreator.Code.Interfaces;
using TerrLauncherPackCreator.Pages.PackCreation;
using TerrLauncherPackCreator.Resources.Localizations;

namespace TerrLauncherPackCreator.Code.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private class ProgressManager : BindableBase, IProgressManager
        {
            private const int UpdateTextDelayMs = 500;
            // ReSharper disable once NotAccessedField.Local
            private readonly Timer _updateTextTimer;

            private string _initialText;
            private string[] _steps;
            private int _step;

            #region backing fields
            private string _text;
            private int _currentProgress;
            private int _maximumProgress;
            private int _remainingFilesCount;
            private bool _isIndeterminate;
            #endregion

            public string Text
            {
                get => _text;
                set
                {
                    if (!SetProperty(ref _text, value))
                        return;

                    _initialText = value;
                    _steps = new[]
                    {
                        _initialText,
                        _initialText + ".",
                        _initialText + "..",
                        _initialText + "...",
                    };
                }
            }
            public int CurrentProgress
            {
                get => _currentProgress;
                set => SetProperty(ref _currentProgress, value);
            }
            public int MaximumProgress
            {
                get => _maximumProgress;
                set => SetProperty(ref _maximumProgress, value);
            }
            public int RemainingFilesCount
            {
                get => _remainingFilesCount;
                set => SetProperty(ref _remainingFilesCount, value);
            }
            public bool IsIndeterminate
            {
                get => _isIndeterminate;
                set => SetProperty(ref _isIndeterminate, value);
            }

            public ProgressManager()
            {
                _updateTextTimer = new Timer(state =>
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

        public IReadonlyProperty<PackCreationViewModel> PackCreationViewModel { get; }

        public IReadonlyProperty<string> WindowTitle { get; }
        public IProperty<int> CurrentStep { get; }
        public IReadonlyProperty<Page[]> StepsPages { get; }

        public IReadonlyProperty<IProgressManager> LoadProgressManager { get; }
        public IReadonlyProperty<IProgressManager> SaveProgressManager { get; }

        public IReadonlyProperty<ObservableCollection<IProgressManager>> ProgressManagers { get; }
        public IReadonlyProperty<IPackProcessor> PackProcessor { get; }

        public IActionCommand GoToPreviousStepCommand { get; }
        public IActionCommand GoToNextStepCommand { get; }

        public MainWindowViewModel()
        {
            WindowTitle = new FieldProperty<string>(Assembly.GetEntryAssembly().GetName().Name).AsReadonly();
            CurrentStep = new FieldProperty<int>(1);

            GoToPreviousStepCommand = new ActionCommand(
                GoToPreviousStepCommand_Execute, 
                GoToPreviousStepCommand_CanExecute
            ).BindCanExecute(CurrentStep);
            GoToNextStepCommand = new ActionCommand(
                GoToNextStepCommand_Execute,
                GoToNextStepCommand_CanExecute
            ).BindCanExecute(CurrentStep);

            LoadProgressManager = new FieldProperty<IProgressManager>(
                new ProgressManager {Text = StringResources.LoadingProgressStep}
            ).AsReadonly();
            SaveProgressManager = new FieldProperty<IProgressManager>(
                new ProgressManager {Text = StringResources.SavingProcessStep}
            ).AsReadonly();

            PackProcessor = new FieldProperty<IPackProcessor>
            {
                Value = new PackProcessor(
                    LoadProgressManager.Value,
                    SaveProgressManager.Value
                )
            }.AsReadonly();

            PackCreationViewModel = new FieldProperty<PackCreationViewModel>(
                new PackCreationViewModel(PackProcessor.Value)
            ).AsReadonly();

            StepsPages = new FieldProperty<Page[]>(
                new Page[]
                {
                    new PackCreationStep1(PackCreationViewModel.Value), 
                    new PackCreationStep2(PackCreationViewModel.Value), 
                    new PackCreationStep3(PackCreationViewModel.Value), 
                    new PackCreationStep4(PackCreationViewModel.Value), 
                }
            ).AsReadonly();

            ProgressManagers = new FieldProperty<ObservableCollection<IProgressManager>>
            (
                new ObservableCollection<IProgressManager>
                {
                    LoadProgressManager.Value,
                    SaveProgressManager.Value
                }
            ).AsReadonly();
        }

        private bool GoToPreviousStepCommand_CanExecute()
        {
            return CurrentStep.Value > 1;
        }

        private void GoToPreviousStepCommand_Execute()
        {
            CurrentStep.Value--;
        }

        private bool GoToNextStepCommand_CanExecute()
        {
            return CurrentStep.Value < StepsPages.Value.Length;
        }

        private void GoToNextStepCommand_Execute()
        {
            CurrentStep.Value++;
        }
    }
}
