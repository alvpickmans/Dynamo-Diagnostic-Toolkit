using System;
using System.Collections.Generic;
using System.Text;

namespace DiagnosticToolkit.Core.Interfaces
{
    public interface IProfilingSession
    {
        Guid Guid { get; }
        TimeSpan ExecutionTime { get; }
        string Name { get; }

        event EventHandler SessionStarted;
        event EventHandler SessionEnded;
        event EventHandler DataAdded;
        event EventHandler DataRemoved;
    }
}
