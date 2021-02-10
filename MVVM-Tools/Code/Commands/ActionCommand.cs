using System;
using MVVM_Tools.Code.Providers;

namespace MVVM_Tools.Code.Commands
{
    /// <summary>
    /// Command that implements <see cref="IActionCommand"/> interface. Parameters is of the <see cref="object"/> type
    /// </summary>
    public class ActionCommand : ActionCommand<object>, IActionCommand
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ActionCommand"/>
        /// </summary>
        /// <param name="executeAction">Action that is called when a command is executed (after <see cref="canExecuteAction"/> is called)</param>
        /// <param name="canExecuteAction">Action that checks parameter reters whether the command can execute</param>
        /// <exception cref="ArgumentNullException">Is thrown if <see cref="executeAction"/> is null</exception>
        public ActionCommand(Action executeAction, Func<bool> canExecuteAction = null)
            : base(
                  parameter => executeAction(),
                  canExecuteAction == null ? (Func<object, bool>)null : parameter => canExecuteAction()
              )
        { }

        /// <summary>
        /// Executes the command with null as a parameter
        /// </summary>
        public void Execute()
        {
            Execute(null);
        }

        public new IActionCommand BindCanExecute<T>(IReadonlyProperty<T> property)
        {
            base.BindCanExecute(property);
            return this;
        }
    }
}
