using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Input;

namespace QuickWindows
{
    /// <remarks>https://stackoverflow.com/questions/2450373/set-global-hotkeys-using-c-sharp</remarks>
    public sealed class KeyboardHook : IDisposable
    {
        /// <summary>
        /// Represents the window that is used internally to get the messages.
        /// </summary>
        class Window : NativeWindow, IDisposable
        {
            private static int WM_HOTKEY = 0x0312;

            readonly Action<KeyPressedEventArgs> _callback;
            public Window(Action<KeyPressedEventArgs> callback)
            {
                _callback = callback;

                // create the handle for the window.
                CreateHandle(new CreateParams());
            }

            /// <summary>
            /// Overridden to get the notifications.
            /// </summary>
            /// <param name="m"></param>
            protected override void WndProc(ref Message m)
            {
                base.WndProc(ref m);

                // check if we got a hot key pressed.
                if (m.Msg == WM_HOTKEY)
                {
                    // get the keys.
                    Keys key = (Keys)(((int)m.LParam >> 16) & 0xFFFF);
                    ModifierKeys modifier = (ModifierKeys)((int)m.LParam & 0xFFFF);

                    // invoke the event to notify the parent.
                    _callback(new KeyPressedEventArgs(modifier, key));
                }
            }

            #region IDisposable Members

            public void Dispose()
            {
                DestroyHandle();
            }

            #endregion
        }

        private Window _window;
        private int _currentId;
        readonly Dictionary<KeyStroke, int> _keyStrokeMap;

        public KeyboardHook()
        {
            // register the event of the inner native window.
            _window = new Window(OnKeyPressed);
            _keyStrokeMap = new Dictionary<KeyStroke, int>();
        }

        /// <summary>
        /// Registers a hot key in the system.
        /// </summary>
        /// <param name="modifier">The modifiers that are associated with the hot key.</param>
        /// <param name="key">The key itself that is associated with the hot key.</param>
        public void RegisterHotKey(KeyStroke keyStroke)
        {
            // increment the counter.
            _currentId++;

            // register the hot key.
            if (!NativeMethods.RegisterHotKey(_window.Handle, _currentId, (uint)keyStroke.ModifierKeys, (uint)keyStroke.GetWinFormsKey()))
                throw new InvalidOperationException("Couldn’t register the hot key.");

            _keyStrokeMap[keyStroke] = _currentId;
        }

        /// <summary>
        /// A hot key has been pressed.
        /// </summary>
        public event EventHandler<KeyPressedEventArgs> KeyPressed;

        void OnKeyPressed(KeyPressedEventArgs args)
        {
            KeyPressed?.Invoke(this, args);
        }

        #region IDisposable Members

        public void Dispose()
        {
            // unregister all the registered hot keys.
            for (int i = _currentId; i > 0; i--)
            {
                NativeMethods.UnregisterHotKey(_window.Handle, i);
            }

            // dispose the inner native window.
            _window.Dispose();
        }

        #endregion
    }
}
