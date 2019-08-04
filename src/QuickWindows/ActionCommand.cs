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
#pragma warning disable 67
        public event EventHandler CanExecuteChanged;
#pragma warning enable 67

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter) => _action();
    }
}
