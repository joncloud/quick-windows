using System;

namespace QuickWindows
{
    /// <remarks>https://github.com/dotnet/corefx/blob/8b73a6dfacccefa8d160077ebff11007ff72fda3/src/System.Diagnostics.Process/src/System/Diagnostics/ProcessManager.Win32.cs#L23</remarks>
    internal sealed class MainWindowFinder
    {
        private const int GW_OWNER = 4;
        private IntPtr _bestHandle;
        private int _processId;

        public IntPtr FindMainWindow(int processId)
        {
            _bestHandle = (IntPtr)0;
            _processId = processId;

            Interop.User32.EnumThreadWindowsCallback callback = new Interop.User32.EnumThreadWindowsCallback(EnumWindowsCallback);
            Interop.User32.EnumWindows(callback, IntPtr.Zero);

            GC.KeepAlive(callback);
            return _bestHandle;
        }

        private bool IsMainWindow(IntPtr handle)
        {
            if (Interop.User32.GetWindow(handle, GW_OWNER) != (IntPtr)0 || !Interop.User32.IsWindowVisible(handle))
                return false;

            return true;
        }

        private bool EnumWindowsCallback(IntPtr handle, IntPtr extraParameter)
        {
            int processId;
            Interop.User32.GetWindowThreadProcessId(handle, out processId);

            if (processId == _processId)
            {
                if (IsMainWindow(handle))
                {
                    _bestHandle = handle;
                    return false;
                }
            }
            return true;
        }
    }
}
