using System;
using System.Runtime.InteropServices;

namespace QuickWindows
{
    /// <remarks>https://github.com/dotnet/corefx/blob/8c5260061b11323dfd97fbab614d54402405513f/src/Common/src/Interop/Windows/User32/Interop.EnumWindows.cs#L8</remarks>
    internal partial class Interop
    {
        internal partial class User32
        {
            internal delegate bool EnumThreadWindowsCallback(IntPtr hWnd, IntPtr lParam);

            [DllImport("user32.dll")]
            public static extern bool EnumWindows(EnumThreadWindowsCallback callback, IntPtr extraData);

            [DllImport("user32.dll")]
            public static extern IntPtr GetWindow(IntPtr hWnd, int uCmd);

            [DllImport("user32.dll")]
            public static extern bool IsWindowVisible(IntPtr hWnd);

            [DllImport("user32.dll", ExactSpelling = true)]
            public static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);
        }
    }
}
