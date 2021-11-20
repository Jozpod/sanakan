using System.Diagnostics;

namespace Sanakan.Common
{
    public interface IOperatingSystem
    {
        /// <inheritdoc cref="Process.GetCurrentProcess"/>
        Process GetCurrentProcess();

        /// <inheritdoc cref="Process.Refresh"/>
        void Refresh(Process process);
    }
}
