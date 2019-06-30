using System;
using System.Collections.Generic;

namespace QuickWindows
{
    class KeyboardShortcuts
    {
        readonly Dictionary<KeyStroke, AppAction> _shortcuts;
        public KeyboardShortcuts()
        {
            _shortcuts = new Dictionary<KeyStroke, AppAction>();
        }

        public event EventHandler<AppActionRequestedEventArgs> AppActionRequested;

        protected virtual void OnAppActionRequested(AppActionRequestedEventArgs args)
        {
            AppActionRequested?.Invoke(this, args);
        }

        public bool TryAdd(KeyStroke keyStroke, AppAction shortcut)
        {
            return _shortcuts.TryAdd(keyStroke, shortcut);
        }

        public void Remove(KeyStroke keyStroke)
        {
            _shortcuts.Remove(keyStroke);
        }

        public bool TryRequest(KeyStroke keyStroke)
        {
            if (_shortcuts.TryGetValue(keyStroke, out var appAction))
            {
                var args = new AppActionRequestedEventArgs(appAction);
                OnAppActionRequested(args);
                return true;
            }
            return false;
        }
    }
}
