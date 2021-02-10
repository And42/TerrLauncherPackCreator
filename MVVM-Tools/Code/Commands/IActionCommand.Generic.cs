using System.Windows.Input;
using MVVM_Tools.Code.Providers;

namespace MVVM_Tools.Code.Commands
{
    public interface IActionCommand<in TParameter> : ICommand
    {
        bool CanExecute(TParameter parameter);

        void Execute(TParameter parameter);

        /// <summary>
        /// Raises <see cref="CanExecuteChanged"/> event
        /// </summary>
        void RaiseCanExecuteChanged();

        IActionCommand<TParameter> BindCanExecute<T>(IReadonlyProperty<T> property);
    }
}