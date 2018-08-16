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

            private string _text;
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

            private int _currentProgress;
            public int CurrentProgress
            {
                get => _currentProgress;
                set => SetProperty(ref _currentProgress, value);
            }

            private int _maximumProgress;
            public int MaximumProgress
            {
                get => _maximumProgress;
                set => SetProperty(ref _maximumProgress, value);
            }

            private int _remainingFilesCount;
            public int RemainingFilesCount
            {
                get => _remainingFilesCount;
                set => SetProperty(ref _remainingFilesCount, value);
            }

            private bool _isIndeterminate;
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

        public Property<PackCreationViewModel> PackCreationViewModel { get; }

        public Property<string> WindowTitle { get; }
        public Property<int> CurrentStep { get; }
        public Property<Page[]> StepsPages { get; }

        public Property<IProgressManager> LoadProgressManager { get; }
        public Property<IProgressManager> SaveProgressManager { get; }

        public Property<ObservableCollection<IProgressManager>> ProgressManagers { get; }
        public Property<IPackProcessor> PackProcessor { get; }

        public IActionCommand GoToPreviousStepCommand { get; }
        public IActionCommand GoToNextStepCommand { get; }

        public MainWindowViewModel()
        {
            WindowTitle = new Property<string>(Assembly.GetEntryAssembly().GetName().Name);
            CurrentStep = new Property<int>(1);

            GoToPreviousStepCommand = new ActionCommand(GoToPreviousStepCommand_Execute, GoToPreviousStepCommand_CanExecute);
            GoToNextStepCommand = new ActionCommand(GoToNextStepCommand_Execute, GoToNextStepCommand_CanExecute);

            LoadProgressManager = new Property<IProgressManager>
            {
                Value = new ProgressManager {Text = StringResources.LoadingProgressStep}
            };
            SaveProgressManager = new Property<IProgressManager>
            {
                Value = new ProgressManager {Text = StringResources.SavingProcessStep}
            };

            PackProcessor = new Property<IPackProcessor>
            {
                Value = new PackProcessor(
                    LoadProgressManager.Value,
                    SaveProgressManager.Value
                )
            };

            PackCreationViewModel = new Property<PackCreationViewModel>
            {
                Value = new PackCreationViewModel(PackProcessor.Value)
            };

            StepsPages = new Property<Page[]>(
                new Page[]
                {
                    new PackCreationStep1(PackCreationViewModel.Value), 
                    new PackCreationStep2(PackCreationViewModel.Value), 
                    new PackCreationStep3(PackCreationViewModel.Value), 
                    new PackCreationStep4(PackCreationViewModel.Value), 
                }
            );

            ProgressManagers = new Property<ObservableCollection<IProgressManager>>
            {
                Value = new ObservableCollection<IProgressManager>
                {
                    LoadProgressManager.Value,
                    SaveProgressManager.Value
                }
            };

            CurrentStep.PropertyChanged += (sender, args) => RaiseNavigationCommandsCanExecute();
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

        private void RaiseNavigationCommandsCanExecute()
        {
            GoToPreviousStepCommand.RaiseCanExecuteChanged();
            GoToNextStepCommand.RaiseCanExecuteChanged();
        }
    }
}
