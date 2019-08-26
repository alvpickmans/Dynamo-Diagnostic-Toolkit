using System;
using System.Collections.Generic;
using System.Text;

namespace DiagnosticToolkit.Core.Interfaces
{
    public interface IProfilingManager<TSession> where TSession : IProfilingSession
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
