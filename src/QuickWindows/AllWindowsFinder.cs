using System;
using System.Collections.Generic;

namespace QuickWindows
{
    class AllWindowsFinder
    {
        public Dictionary<int, IntPtr> MainWindows { get; } = new Dictionary<int, IntPtr>();

        public AllWindowsFinder()
        {
            Refresh();
        }

        public void Refresh()
        {
            MainWindows.Clear();

            Interop.User32.EnumThreadWindowsCallback callback = new Interop.User32.EnumThreadWindowsCallback(EnumWindowsCallback);
            Interop.User32.EnumWindows(callback, IntPtr.Zero);

            GC.KeepAlive(callback);
        }

        const int GW_OWNER = 4;

        bool IsMainWindow(IntPtr handle)
        {
            if (Interop.User32.GetWindow(handle, GW_OWNER) != (IntPtr)0 || !Interop.User32.IsWindowVisible(handle))
                return false;

            return true;
        }

        private bool EnumWindowsCallback(IntPtr handle, IntPtr _)
        {
            Interop.User32.GetWindowThreadProcessId(handle, out var processId);

            if (IsMainWindow(handle))
            {
                MainWindows[processId] = handle;
            }

            return true;
        }
    }
}
