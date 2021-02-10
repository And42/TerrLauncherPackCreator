using MVVM_Tools.Code.Providers;

namespace MVVM_Tools.Code.Commands
{
    public interface IActionCommand : IActionCommand<object>
    {
        /// <summary>
        /// Executes the command with null as a parameter
        /// </summary>
        void Execute();

        new IActionCommand BindCanExecute<T>(IReadonlyProperty<T> property);
    }
}