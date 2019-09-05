using DiagnosticToolkit.Core.Interfaces;
using DiagnosticToolkit.UI.Models;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Helpers;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Threading;

namespace DiagnosticToolkit.UI.ViewModels
{
    public class DiagnosticMainViewModel : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Dispatcher CallingDispatcher = Dispatcher.CurrentDispatcher;

        private IProfilingManager manager;
        private IProfilingSession session;

        public string SessionName { get; set; }

        public NoisyCollection<ProfilingDataPoint> NodeProfilingData { get; private set; }

        public DiagnosticMainViewModel(IProfilingManager manager)
        {
            this.manager = manager ?? throw new ArgumentNullException(nameof(manager));
            this.NodeProfilingData = new NoisyCollection<ProfilingDataPoint>();

            this.RegisterSession(this.manager.CurrentSession);
            this.RegisterEvents();
        }

        public void Dispose()
        {
            this.UnregisterEvents();
        }

        #region Events

        private void RegisterEvents()
        {
            this.manager.SessionChanged += this.RegisterSession;
        }

        private void UnregisterEvents()
        {
            this.manager.SessionChanged -= this.RegisterSession;
        }

        private void RegisterSessionEvents(IProfilingSession session)
        {
            if (this.session == null)
                return;

            this.session.SessionStarted += this.OnSessionStarted;
            this.session.SessionEnded += this.OnSessionEnded;
            this.session.DataAdded += this.OnDataAdded;
            this.session.DataRemoved += this.OnDataRemoved;
        }

        private void UnregisterSessionEvents(IProfilingSession session)
        {
            if (this.session == null)
                return;

            this.session.SessionStarted -= this.OnSessionStarted;
            this.session.SessionEnded -= this.OnSessionEnded;
            this.session.DataAdded -= this.OnDataAdded;
            this.session.DataRemoved -= this.OnDataRemoved;
        }

        private void RegisterSession(IProfilingSession session)
        {
            this.UnregisterSessionEvents(this.session);
            this.session = session;

            if (this.session != null && this.session.ProfilingData.Any())
            {
                var dataPoints = this.session.ProfilingData.Select(data => new ProfilingDataPoint(data));
                this.NodeProfilingData = new NoisyCollection<ProfilingDataPoint>(dataPoints);
            }

            this.RegisterSessionEvents(this.session);
        }
        private void OnSessionStarted(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void OnSessionEnded(object sender, EventArgs e)
        {
            CallingDispatcher.Invoke(() =>
            {
                foreach (var dataPoint in this.NodeProfilingData)
                {
                    dataPoint.UpdateWeight();
                }
            });        
        }

        private void OnDataRemoved(IProfilingData data)
        {
            var instance = this.NodeProfilingData.FirstOrDefault(d => d.Instance.Equals(data));
            if (instance != null)
                this.NodeProfilingData.Remove(instance);
        }

        private void OnDataAdded(IProfilingData data)
        {
            this.NodeProfilingData.Add(new ProfilingDataPoint(data));
        }


        #endregion Events
    }
}