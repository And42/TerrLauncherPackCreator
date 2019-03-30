using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Threading;
using System.Windows.Controls;
using MVVM_Tools.Code.Classes;
using MVVM_Tools.Code.Commands;
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

        public PackCreationViewModel PackCreationViewModel { get; }

        public string WindowTitle { get; }
        public int CurrentStep
        {
            get => _currentStep;
            set => SetProperty(ref _currentStep, value);
        }
        public Page[] StepsPages { get; }

        public IProgressManager LoadProgressManager { get; }
        public IProgressManager SaveProgressManager { get; }

        public ObservableCollection<IProgressManager> ProgressManagers { get; }
        public IPackProcessor PackProcessor { get; }

        public IActionCommand GoToPreviousStepCommand { get; }
        public IActionCommand GoToNextStepCommand { get; }

        #region backing fields
        private int _currentStep;
        #endregion

        public MainWindowViewModel()
        {
            WindowTitle = Assembly.GetEntryAssembly().GetName().Name;
            CurrentStep = 1;

            GoToPreviousStepCommand = new ActionCommand(
                GoToPreviousStepCommand_Execute, 
                GoToPreviousStepCommand_CanExecute
            );
            GoToNextStepCommand = new ActionCommand(
                GoToNextStepCommand_Execute,
                GoToNextStepCommand_CanExecute
            );

            LoadProgressManager = new ProgressManager {Text = StringResources.LoadingProgressStep};
            SaveProgressManager = new ProgressManager {Text = StringResources.SavingProcessStep};

            PackProcessor = new PackProcessor(
                LoadProgressManager,
                SaveProgressManager
            );

            PackCreationViewModel = new PackCreationViewModel(PackProcessor);

            StepsPages = new Page[]
            {
                new PackCreationStep1(PackCreationViewModel), 
                new PackCreationStep2(PackCreationViewModel), 
                new PackCreationStep3(PackCreationViewModel), 
                new PackCreationStep4(PackCreationViewModel), 
            };

            ProgressManagers = new ObservableCollection<IProgressManager>
            {
                LoadProgressManager,
                SaveProgressManager
            };

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

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
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
}
