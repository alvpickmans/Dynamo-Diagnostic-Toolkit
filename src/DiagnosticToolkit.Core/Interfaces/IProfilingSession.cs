using System;
using System.Collections.Generic;
using System.Text;

namespace DiagnosticToolkit.Core.Interfaces
{
    public interface IProfilingSession
    {
        TimeSpan ExecutionTime { get; }

        event EventHandler SessionStarted;
        event EventHandler SessionEnded;
        event EventHandler DataAdded;
        event EventHandler DataRemoved;
    }
}
