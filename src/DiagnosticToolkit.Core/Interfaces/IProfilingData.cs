using System;
using System.Collections.Generic;
using System.Text;

namespace DiagnosticToolkit.Core.Interfaces
{
    public interface IProfilingData
    {
        TimeSpan ExecutionTime { get; }
        bool CanRequestExecution { get; }

        bool HasExecutionPending { get; }

        bool Executed { get; }

        string Name { get; }
        string Id { get; }

        double X { get; }
        double Y { get; }

        void RequestExecution();

        event Action<IProfilingData> Modified;

        event Action<IProfilingData> PositionChanged;
        event Action<IProfilingData> ProfilingStarted;
        event Action<IProfilingData> ProfilingEnded;
    }
}
