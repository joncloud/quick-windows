using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace QuickWindows
{
    public class ProcessService : IProcessService
    {
        readonly Process _currentProcess = Process.GetCurrentProcess();
        public IEnumerable<WindowProcess> GetWindowProcesses() =>
            WindowProcess.FromProcesses()
                .Where(process => process.ProcessId != _currentProcess.Id)
                .OrderBy(process => process.ProcessName)
                .ThenBy(process => process.MainWindowTitle);
    }
}
