using System;
using System.Collections.Generic;
using System.Text;

namespace DiagnosticToolkit.Core.Interfaces
{
    public interface IProfilingManager<TSession, TData>
        where TSession : IProfilingSession<TData>
        where TData : IProfilingData
    {
        /// <summary>
        /// Current Profiling session.
        /// </summary>
        TSession CurrentSession { get; }

        /// <summary>
        /// Enable profiling.
        /// </summary>
        void EnableProfiling();

        /// <summary>
        /// Disable profiling.
        /// </summary>
        void DisableProfiling();
    }
}
