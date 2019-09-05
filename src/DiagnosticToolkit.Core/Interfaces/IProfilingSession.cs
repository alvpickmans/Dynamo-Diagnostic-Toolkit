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
        IEnumerable<IProfilingData> ProfilingData { get; }
        bool Executing { get; }

        event EventHandler SessionStarted;
        event EventHandler SessionEnded;
        event EventHandler SessionCleared;
        event Action<IProfilingData> DataAdded;
        event Action<IProfilingData> DataRemoved;
    }
}
