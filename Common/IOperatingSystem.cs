using System.Diagnostics;

namespace Sanakan.Common
{
    /// <summary>
    /// Provides an abstraction for operating system methods.
    /// </summary>
    public interface IOperatingSystem
    {
        /// <inheritdoc cref="Process.GetCurrentProcess"/>
        Process GetCurrentProcess();

        /// <inheritdoc cref="Process.Refresh"/>
        void Refresh(Process process);
    }
}
