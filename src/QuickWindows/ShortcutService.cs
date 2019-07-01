using System;
using System.Collections.Generic;
using System.Linq;

namespace QuickWindows
{
    class ShortcutService : IShortcutService
    {
        readonly Dictionary<KeyStroke, AppAction> _shortcuts;
        KeyboardHook _keyboardHook;
        public ShortcutService()
        {
            _shortcuts = new Dictionary<KeyStroke, AppAction>();
            InitKeyboardHook();
        }

        void InitKeyboardHook()
        {
            _keyboardHook = new KeyboardHook();
            foreach (var pair in _shortcuts)
            {
                if (IsGlobalShortcut(pair.Value))
                {
                    _keyboardHook.RegisterHotKey(pair.Key);
                }
            }
            _keyboardHook.KeyPressed += HandleKeyboardHookKeyPressed;
        }

        void HandleKeyboardHookKeyPressed(object sender, KeyPressedEventArgs e)
        {
            if (KeyStroke.TryConvert(e, out var keyStroke))
            {
                TryRequestShortcut(keyStroke);
            }
        }

        public event EventHandler<AppActionRequestedEventArgs> AppActionRequested;

        protected virtual void OnAppActionRequested(AppActionRequestedEventArgs args)
        {
            AppActionRequested?.Invoke(this, args);
        }

        static bool IsGlobalShortcut(AppAction shortcut) =>
            shortcut == AppAction.ActivateApp;

        public IEnumerable<(KeyStroke, AppAction)> GetShortcuts() =>
            _shortcuts.Select(pair => (pair.Key, pair.Value));

        public bool TryAddShortcut(KeyStroke keyStroke, AppAction shortcut)
        {
            if (_shortcuts.TryAdd(keyStroke, shortcut))
            {
                if (IsGlobalShortcut(shortcut))
                {
                    _keyboardHook.RegisterHotKey(keyStroke);
                }
                return true;
            }
            return false;
        }

        public void RemoveShortcut(KeyStroke keyStroke)
        {
            if (_shortcuts.Remove(keyStroke, out var shortcut))
            {
                if (IsGlobalShortcut(shortcut))
                {
                    _keyboardHook.Dispose();
                    InitKeyboardHook();
                }
            }
        }

        public bool TryRequestShortcut(KeyStroke keyStroke)
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
