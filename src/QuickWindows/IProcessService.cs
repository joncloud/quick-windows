using System.Collections.Generic;

namespace QuickWindows
{
    public interface IProcessService
    {
        IEnumerable<WindowProcess> GetWindowProcesses();
    }
}
