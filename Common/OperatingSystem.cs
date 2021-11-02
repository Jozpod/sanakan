using System.Diagnostics;

namespace Sanakan.Common
{
    internal class OperatingSystem : IOperatingSystem
    {
        public Process GetCurrentProcess() => Process.GetCurrentProcess();

        public void Refresh(Process process) => process.Refresh();
    }
}
