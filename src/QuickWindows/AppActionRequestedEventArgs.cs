using System;

namespace QuickWindows
{
    class AppActionRequestedEventArgs : EventArgs
    {
        public AppAction AppAction { get; }
        public AppActionRequestedEventArgs(AppAction appAction) =>
            AppAction = appAction;
    }
}
