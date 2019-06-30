using System;
using System.Windows.Input;

namespace QuickWindows
{
    public class ActionCommand : ICommand
    {
        readonly Action _action;
        public ActionCommand(Action action)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action));
        }
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter) => _action();
    }
}
