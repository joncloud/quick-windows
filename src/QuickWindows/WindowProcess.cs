using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Media;
using System.Collections.Concurrent;

namespace QuickWindows
{
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
                if (mainWindows.TryGetValue(process.Id, out var mainWindow) &&
                   !string.IsNullOrWhiteSpace(process.MainWindowTitle))
                {
                    yield return new WindowProcess(process, mainWindow);
                }
            }
        }
    }
}
