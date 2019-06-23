using System;
using System.Diagnostics;
using System.Text;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace QuickWindows
{
    /// <remarks>http://www.aboutmycode.com/net-framework/how-to-get-elevated-process-path-in-net/</remarks>
    static class ProcessExtensions
    {
        public static string GetExecutablePath(this Process process)
        { 
            if (Environment.OSVersion.Version.Major >= 6)
            {
                return GetExecutablePathAboveVista(process.Id);
            }

            return process.MainModule.FileName;
        }

        static string GetExecutablePathAboveVista(int processId)
        {
            var buffer = new StringBuilder(1024);
            IntPtr hprocess = NativeMethods.OpenProcess(ProcessAccessFlags.PROCESS_QUERY_LIMITED_INFORMATION,
                                          false, processId);
            if (hprocess != IntPtr.Zero)
            {
                try
                {
                    int size = buffer.Capacity;
                    if (NativeMethods.QueryFullProcessImageName(hprocess, 0, buffer, out size))
                    {
                        return buffer.ToString();
                    }
                }
                finally
                {
                    NativeMethods.CloseHandle(hprocess);
                }
            }
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }
    }
}
