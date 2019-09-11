using System;
using System.Collections.Generic;
using System.Text;

namespace DiagnosticToolkit.Core.Interfaces
{
    public interface IProfilingData
    {
        TimeSpan ExecutionTime { get; }

        bool Executed { get; }

        string Name { get; }
        string Id { get; }

        double X { get; }
        double Y { get; }

        event Action<IProfilingData> PositionChanged;
        event Action<IProfilingData> ProfilingExecuted;
    }
}
