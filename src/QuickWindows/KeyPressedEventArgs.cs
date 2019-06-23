using System;
using System.Windows.Forms;

namespace QuickWindows
{
    /// <summary>
    /// Event Args for the event that is fired after the hot key has been pressed.
    /// </summary>
    /// <remarks>https://stackoverflow.com/questions/2450373/set-global-hotkeys-using-c-sharp</remarks>
    public class KeyPressedEventArgs : EventArgs
    {
        internal KeyPressedEventArgs(ModifierKeys modifier, Keys key)
        {
            Modifier = modifier;
            Key = key;
        }

        public ModifierKeys Modifier { get; }

        public Keys Key { get; }
    }
}
