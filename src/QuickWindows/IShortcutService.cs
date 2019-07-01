using System;
using System.Collections.Generic;

namespace QuickWindows
{
    public interface IShortcutService
    {
        event EventHandler<AppActionRequestedEventArgs> AppActionRequested;

        IEnumerable<(KeyStroke, AppAction)> GetShortcuts();
        void RemoveShortcut(KeyStroke keyStroke);
        bool TryAddShortcut(KeyStroke keyStroke, AppAction appAction);
        bool TryRequestShortcut(KeyStroke keyStroke);
    }
}
