using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

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
