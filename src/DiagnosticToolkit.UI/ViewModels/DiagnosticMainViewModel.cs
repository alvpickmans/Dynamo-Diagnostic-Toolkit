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
using System.Windows.Data;
using System.Windows.Threading;
using PropertyChanged;
using LiveCharts.Configurations;
using TinyLittleMvvm;
using System.Windows.Input;

namespace DiagnosticToolkit.UI.ViewModels
{
    public class DiagnosticMainViewModel : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region Properties
        private Dispatcher CallingDispatcher = Dispatcher.CurrentDispatcher;

        private IProfilingManager manager;
        private IProfilingSession session;

        public string SessionName { get; set; }

        [AlsoNotifyFor(nameof(NodeProfilingData))]
        public double MinimumExecutionTime { get; set; }

        [AlsoNotifyFor(nameof(NodeProfilingData))]
        public double MaximumExecutionTime { get; set; }

        [AlsoNotifyFor(nameof(NodeProfilingData))]
        public double LowerTimeRange { get; set; }

        [AlsoNotifyFor(nameof(NodeProfilingData))]
        public double UpperTimeRange { get; set; }

        public ChartValues<ProfilingDataPoint> NodeProfilingData { get; private set; }

        public WeightedMapper<ProfilingDataPoint> Mapper => ChartMappers.ProfilingDataPointMapper;

        public ProfilingDataPoint SelectedData { get; set; } 
        #endregion

        public DiagnosticMainViewModel(IProfilingManager manager)
        {
            this.manager = manager ?? throw new ArgumentNullException(nameof(manager));

            this.RegisterSession(this.manager.CurrentSession);
            this.RegisterEvents();
        }

        private void NodeProfilingDataFilter(object sender, FilterEventArgs e)
        {
            ProfilingDataPoint dataPoint = e.Item as ProfilingDataPoint;

            if (dataPoint == null)
                e.Accepted = false;
            else
                e.Accepted = dataPoint.Weight >= this.LowerTimeRange && dataPoint.Weight <= this.UpperTimeRange;
        }

        public void Dispose()
        {
            this.UnregisterEvents();
            this.UnregisterSessionEvents(this.session);
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
            this.session.SessionCleared += this.OnSessionCleared;
            this.session.DataAdded += this.OnDataAdded;
            this.session.DataRemoved += this.OnDataRemoved;
        }

        private void UnregisterSessionEvents(IProfilingSession session)
        {
            if (this.session == null)
                return;

            this.session.SessionStarted -= this.OnSessionStarted;
            this.session.SessionEnded -= this.OnSessionEnded;
            this.session.SessionCleared -= this.OnSessionCleared;
            this.session.DataAdded -= this.OnDataAdded;
            this.session.DataRemoved -= this.OnDataRemoved;
        }

        private void RegisterSession(IProfilingSession session)
        {
            this.UnregisterSessionEvents(this.session);
            this.session = session;
            this.SessionName = session?.Name;
            this.NodeProfilingData = new ChartValues<ProfilingDataPoint>();

            if (this.session != null && this.session.ProfilingData.Any())
            {
                var dataPoints = this.session.ProfilingData.Select(data => new ProfilingDataPoint(data));
                this.NodeProfilingData.AddRange(dataPoints);
            }

            this.RegisterSessionEvents(this.session);
        }
        private void OnSessionStarted(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void OnSessionCleared(object sender, EventArgs e)
        {
            this.NodeProfilingData.Clear();
        }

        private void OnSessionEnded(object sender, EventArgs e)
        {
            double min = Double.PositiveInfinity;
            double max = 0;
            CallingDispatcher.Invoke(() =>
            {
                foreach (var dataPoint in this.NodeProfilingData)
                {
                    dataPoint.UpdateWeight();

                    if (dataPoint.Weight < min) min = dataPoint.Weight;
                    if (dataPoint.Weight > max) max = dataPoint.Weight;
                }

                this.MinimumExecutionTime = min;
                this.MaximumExecutionTime = max;
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

        #region Commands

        private void InitializeCommands()
        {
            //this.ForceExecutionCommand = new RelayCommand(ForceExecutionExecute, CanForceExecution);
        }

        public ICommand ForceExecutionCommand => new RelayCommand(ForceExecutionExecute, CanForceExecution);

        public bool CanForceExecution() => this.NodeProfilingData.Any(data => data.Instance.CanScheduleExecution && !data.Instance.HasExecutionPending);

        public void ForceExecutionExecute()
        {
            foreach (var data in this.NodeProfilingData)
            {
                data.ForceExecution();
            }
        }

        #endregion
    }
}