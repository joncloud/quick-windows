using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Media;
using System.Collections.Concurrent;
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

    public class WindowProcess
    {
        readonly Process _process;
        readonly IntPtr _mainWindow;
        static ConcurrentDictionary<string, ImageSource> _imageCache;
        static WindowProcess()
        {
            _imageCache = new ConcurrentDictionary<string, ImageSource>(StringComparer.OrdinalIgnoreCase);
        }
        WindowProcess(Process process, IntPtr mainWindow)
        {
            _process = process ?? throw new ArgumentNullException(nameof(process));
            _mainWindow = mainWindow;
            ProcessImage = _imageCache.GetOrAdd(
                process.GetExecutablePath(),
                path => Icon.ExtractAssociatedIcon(path).ToImageSource()
            );
        }

        public string ProcessName => _process.ProcessName;
        public string MainWindowTitle => _process.MainWindowTitle;
        public int ProcessId => _process.Id;
        public ImageSource ProcessImage { get; }

        public bool TryFocusMainWindow()
        {
            NativeMethods.SetForegroundWindow(_mainWindow);
            return true;
        }

        static readonly AllWindowsFinder _allWindowsFinder = new AllWindowsFinder();
        public static IEnumerable<WindowProcess> FromProcesses()
        {
            _allWindowsFinder.Refresh();

            var mainWindows = _allWindowsFinder.MainWindows;
            foreach (var process in Process.GetProcesses())
            {
                if (mainWindows.TryGetValue(process.Id, out var mainWindow))
                {
                    yield return new WindowProcess(process, mainWindow);
                }
            }
        }
    }
}
