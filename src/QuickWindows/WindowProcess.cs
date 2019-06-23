using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Media;

namespace QuickWindows
{
    public class WindowProcess
    {
        readonly Process _process;
        WindowProcess(Process process)
        {
            _process = process ?? throw new ArgumentNullException(nameof(process));
            ProcessImage = Icon.ExtractAssociatedIcon(process.MainModule.FileName).ToImageSource();
        }

        public string ProcessName => _process.ProcessName;
        public string MainWindowTitle => _process.MainWindowTitle;
        public int ProcessId => _process.Id;
        public ImageSource ProcessImage { get; }

        public bool TryFocusMainWindow()
        {
            _process.Refresh();
            if (_process.MainWindowHandle != IntPtr.Zero)
            {
                NativeMethods.SetForegroundWindow(_process.MainWindowHandle);
                return true;
            }
            return false;
        }

        public static IEnumerable<WindowProcess> FromProcesses() =>
            Process.GetProcesses()
                .Where(process => process.MainWindowHandle != IntPtr.Zero)
                .Select(process => new WindowProcess(process))
                .OrderBy(process => process.ProcessName)
                .ThenBy(process => process.MainWindowTitle);
    }
}
