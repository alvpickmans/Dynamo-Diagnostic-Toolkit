using System;
using System.Collections.Generic;
using System.Text;

namespace DiagnosticToolkit.Core.Interfaces
{
    public interface IProfilingManager
    {
        /// <summary>
        /// Current Profiling session.
        /// </summary>
        IProfilingSession CurrentSession { get; }

        /// <summary>
        /// Enable profiling.
        /// </summary>
        void EnableProfiling();

        /// <summary>
        /// Disable profiling.
        /// </summary>
        void DisableProfiling();

        event Action<IProfilingSession> SessionChanged;
        event Action ProfilingStarted;
        event Action ProfilingEnded;
    }
}
