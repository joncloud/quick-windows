using System;
using System.Collections.Generic;

namespace QuickWindows
{
    public class DesignerShortcutService : IShortcutService
    {
        public event EventHandler<AppActionRequestedEventArgs> AppActionRequested;

        public IEnumerable<(KeyStroke, AppAction)> GetShortcuts()
        {
            yield break;
        }

        public void RemoveShortcut(KeyStroke keyStroke) { }
        public bool TryAddShortcut(KeyStroke keyStroke, AppAction appAction) => false;
        public bool TryRequestShortcut(KeyStroke keyStroke) => false;
    }
}
