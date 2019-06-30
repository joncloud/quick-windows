using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace QuickWindows
{
    /// <remarks>http://www.aboutmycode.com/net-framework/how-to-get-elevated-process-path-in-net/</remarks>
    static class ProcessExtensions
    {
        static readonly ObjectPool<StringBuilder> _stringBuilders;
        static ProcessExtensions()
        {
            const int INITIAL_CAPACITY = 1024;
            _stringBuilders = new ObjectPool<StringBuilder>(
                Enumerable.Range(0, 10)
                    .Select(_ => new StringBuilder(INITIAL_CAPACITY)),
                () => new StringBuilder(INITIAL_CAPACITY)
            );
        }

        public static string GetExecutablePath(this Process process)
        {
            return GetExecutablePathAboveVista(process.Id);
        }

        static string GetExecutablePathAboveVista(int processId)
        {
            IntPtr hprocess = NativeMethods.OpenProcess(ProcessAccessFlags.PROCESS_QUERY_LIMITED_INFORMATION,
                                          false, processId);
            if (hprocess != IntPtr.Zero)
            {
                var buffer = _stringBuilders.Rent();
                try
                {
                    int size = buffer.Capacity;
                    if (NativeMethods.QueryFullProcessImageNameA(hprocess, 0, buffer, out size))
                    {
                        return buffer.ToString();
                    }
                }
                finally
                {
                    NativeMethods.CloseHandle(hprocess);
                    buffer.Clear();
                    _stringBuilders.Return(buffer);
                }
            }
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }
    }
}
