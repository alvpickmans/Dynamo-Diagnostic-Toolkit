using DiagnosticToolkit.Core.Interfaces;
using System;
using System.ComponentModel;

namespace DiagnosticToolkit.UI.ViewModels
{
    public class DiagnosticMainViewModel : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private IProfilingManager manager;

        public string SessionName { get; set; }

        public DiagnosticMainViewModel(IProfilingManager manager)
        {
            this.manager = manager ?? throw new ArgumentNullException(nameof(manager));

            this.RegisterEvents();
        }

        public void Dispose()
        {
            this.UnregisterEvents();
        }

        #region Events

        private void RegisterEvents()
        {
            this.manager.SessionChanged += this.OnSessionChanged;
            this.manager.ProfilingStarted += this.OnProfilingStarted;
            this.manager.ProfilingEnded += this.OnProfilingEnded;
        }

        private void UnregisterEvents()
        {
            this.manager.ProfilingStarted -= this.OnProfilingStarted;
            this.manager.ProfilingEnded -= this.OnProfilingEnded;
        }

        private void OnSessionChanged(IProfilingSession session)
        {
            throw new NotImplementedException();
        }

        private void OnProfilingStarted()
        {
            throw new NotImplementedException();
        }

        private void OnProfilingEnded()
        {
            throw new NotImplementedException();
        }

        #endregion Events
    }
}