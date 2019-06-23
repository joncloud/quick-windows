using System;

namespace QuickWindows
{
    /// <summary>
    /// The enumeration of possible modifiers.
    /// </summary>
    /// <remarks>https://stackoverflow.com/questions/2450373/set-global-hotkeys-using-c-sharp</remarks>
    [Flags]
    public enum ModifierKeys : uint
    {
        Alt = 1,
        Control = 2,
        Shift = 4,
        Win = 8
    }
}
