using System;

namespace QuickWindows
{
    public class AppActionRequestedEventArgs : EventArgs
    {
        public AppAction AppAction { get; }
        public AppActionRequestedEventArgs(AppAction appAction) =>
            AppAction = appAction;
    }
}
