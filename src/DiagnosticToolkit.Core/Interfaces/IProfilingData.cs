using System;
using System.Collections.Generic;
using System.Text;

namespace DiagnosticToolkit.Core.Interfaces
{
    public interface IProfilingData
    {
        TimeSpan ExecutionTime { get; }

        double X { get; }
        double Y { get; }
    }
}
