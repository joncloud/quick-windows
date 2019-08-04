using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Media;

namespace QuickWindows
{
    public class WindowProcess
    {
        readonly Process _process;
        readonly IntPtr _mainWindow;
        static ConcurrentDictionary<string, ImageSource> _imageCache;
        static ConcurrentDictionary<string, string> _executableNames;
        static WindowProcess()
        {
            _imageCache = new ConcurrentDictionary<string, ImageSource>(StringComparer.OrdinalIgnoreCase);
            _executableNames = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }
        WindowProcess(Process process, IntPtr mainWindow)
        {
            _process = process ?? throw new ArgumentNullException(nameof(process));
            _mainWindow = mainWindow;
            var executablePath = process.GetExecutablePath();
            ProcessImage = _imageCache.GetOrAdd(
                executablePath,
                path => Icon.ExtractAssociatedIcon(path).ToImageSource()
            );
            ExecutableName = _executableNames.GetOrAdd(
                executablePath,
                Path.GetFileName
            );
        }

        public string ExecutableName { get; }
        public string ProcessName => _process.ProcessName;
        public string MainWindowTitle => _process.MainWindowTitle;
        public int ProcessId => _process.Id;
        public ImageSource ProcessImage { get; }

        public bool TryFocusMainWindow()
        {
            SetForegroundWindowEx(_mainWindow);
            return true;
        }

        static bool SetForegroundWindowEx(IntPtr hWnd)
        {
            // Don't attempt to focus to a hung window.
            if (NativeMethods.IsHungAppWindow(hWnd))
            {
                return false;
            }

            // https://github.com/ytakanashi/Tascher/blob/d653a93c26ab78e4efb66e4a2b9d474342db6ad4/Src/Function.cpp#L444-L449
            // Send a message to restore the window instead of applying foreground focus.
            if (NativeMethods.IsIconic(hWnd))
            {
                const int WM_SYSCOMMAND = 0x0112;
                const int SC_RESTORE = 0xf120;
                NativeMethods.SendMessage(hWnd, WM_SYSCOMMAND, new IntPtr(SC_RESTORE), IntPtr.Zero);

                return true;
            }

            // https://github.com/ytakanashi/Tascher/blob/d653a93c26ab78e4efb66e4a2b9d474342db6ad4/Src/Function.cpp#L466-L467
            // Attempt to bring the window to the top, and failing set the window to the foreground.
            if (!NativeMethods.BringWindowToTop(hWnd))
            {
                return NativeMethods.SetForegroundWindow(hWnd);
            }

            return false;
        }

        static readonly AllWindowsFinder _allWindowsFinder = new AllWindowsFinder();

        // Ideally there would be an attribute to the process instead of looking at the program name,
        // but there does not appear to be anything unique to identify.
        static readonly HashSet<string> _ignoredExecutables = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "SystemSettings.exe",
            "WindowsInternal.ComposableShell.Experiences.TextInput.InputApp.exe"
        };
        public static IEnumerable<WindowProcess> FromProcesses()
        {
            _allWindowsFinder.Refresh();

            var mainWindows = _allWindowsFinder.MainWindows;
            foreach (var process in Process.GetProcesses())
            {
                if (mainWindows.TryGetValue(process.Id, out var mainWindow) &&
                   !string.IsNullOrWhiteSpace(process.MainWindowTitle))
                {
                    var windowProcess = new WindowProcess(process, mainWindow);

                    if (!_ignoredExecutables.Contains(windowProcess.ExecutableName))
                    {
                        yield return windowProcess;
                    }
                }
            }
        }
    }
}
