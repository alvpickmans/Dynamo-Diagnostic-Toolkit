using System;
using System.Collections.Generic;
using System.Text;

namespace DiagnosticToolkit.Core.Interfaces
{
    public interface IProfilingSession<TData> where TData : IProfilingData
    {
        Guid Guid { get; }
        TimeSpan ExecutionTime { get; }
        string Name { get; }
        IEnumerable<TData> ProfilingData { get; }
        bool Executing { get; }

        event EventHandler SessionStarted;
        event EventHandler SessionEnded;
        event EventHandler DataAdded;
        event EventHandler DataRemoved;
    }
}
