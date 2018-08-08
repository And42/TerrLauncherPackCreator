using System.Reflection;
using System.Windows.Controls;
using MVVM_Tools.Code.Classes;
using MVVM_Tools.Code.Commands;
using MVVM_Tools.Code.Providers;
using TerrLauncherPackCreator.Pages.PackCreation;

namespace TerrLauncherPackCreator.Code.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        public Property<string> WindowTitle { get; }
        public Property<int> CurrentStep { get; }
        public Property<Page[]> StepsPages { get; }

        public IActionCommand GoToPreviousStepCommand { get; }
        public IActionCommand GoToNextStepCommand { get; }

        public MainWindowViewModel()
        {
            var packCreationViewModel = new PackCreationViewModel();

            WindowTitle = new Property<string>(Assembly.GetEntryAssembly().GetName().Name);
            // todo: change to 1
            CurrentStep = new Property<int>(1);

            StepsPages = new Property<Page[]>(
                new Page[]
                {
                    new PackCreationStep1(packCreationViewModel), 
                    new PackCreationStep2(packCreationViewModel), 
                }
            );

            GoToPreviousStepCommand = new ActionCommand(GoToPreviousStepCommand_Execute, GoToPreviousStepCommand_CanExecute);
            GoToNextStepCommand = new ActionCommand(GoToNextStepCommand_Execute, GoToNextStepCommand_CanExecute);
        }

        private bool GoToPreviousStepCommand_CanExecute()
        {
            return CurrentStep.Value > 1;
        }

        private void GoToPreviousStepCommand_Execute()
        {
            CurrentStep.Value--;
            RaiseNavigationCommandsCanExecute();
        }

        private bool GoToNextStepCommand_CanExecute()
        {
            return CurrentStep.Value < StepsPages.Value.Length;
        }

        private void GoToNextStepCommand_Execute()
        {
            CurrentStep.Value++;
            RaiseNavigationCommandsCanExecute();
        }

        private void RaiseNavigationCommandsCanExecute()
        {
            GoToPreviousStepCommand.RaiseCanExecuteChanged();
            GoToNextStepCommand.RaiseCanExecuteChanged();
        }
    }
}
