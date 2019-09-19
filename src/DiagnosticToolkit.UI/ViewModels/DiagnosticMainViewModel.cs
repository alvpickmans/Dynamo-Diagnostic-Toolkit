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
using System.Collections;

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

        public ProfilingDataPoint SelectedNode { get; set; }

        public WeightedMapper<ProfilingDataPoint> Mapper => ChartMappers.ProfilingDataPointMapper;
        #endregion

        public DiagnosticMainViewModel(IProfilingManager manager)
        {
            this.manager = manager ?? throw new ArgumentNullException(nameof(manager));

            this.RegisterSession(this.manager.CurrentSession);
            this.RegisterEvents();

            this.InitializeCommands();
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
            this.SessionName = this.session.Name;
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

        public void InitializeCommands()
        {
            this.RequestAllExecutionCommand = new RelayCommand(RequestAllExecutionExecute, CanRequestAllExecution);
            this.RequestNodesExecutionCommand = new RelayCommand<IList>(RequestNodesExecutionExecute, CanRequestNodesExecution);
            this.DataPointClickCommand = new RelayCommand<ChartPoint>(DataPointClickExecute);
            this.ChartDoubleClickCommand = new RelayCommand(ChartDoubleClickExecute);
            this.SelectionChangedCommand = new RelayCommand<IList>(SelectionChangedExecute);
        }

        public ICommand RequestAllExecutionCommand { get; private set; }

        public bool CanRequestAllExecution() => this.NodeProfilingData.Any(data => data.Instance.CanRequestExecution && !data.Instance.HasExecutionPending);

        public void RequestAllExecutionExecute()
        {
            foreach (var data in this.NodeProfilingData)
            {
                data.ForceExecution();
            }
        }

        public ICommand RequestNodesExecutionCommand { get; private set; }
        public bool CanRequestNodesExecution(IList obj) => obj.OfType<ProfilingDataPoint>().Any(data => !data.Instance.HasExecutionPending);
        public void RequestNodesExecutionExecute(IList obj)
        {
            var dataPoints = obj.OfType<ProfilingDataPoint>();

            foreach (var dataPoint in dataPoints)
            {
                if(!dataPoint.Instance.HasExecutionPending)
                    dataPoint.ForceExecution();
            }
        }

        public ICommand DataPointClickCommand { get; private set; }
        public void DataPointClickExecute(ChartPoint dataPoint)
        {
            if (dataPoint.Instance is ProfilingDataPoint data)
            {
                data.Selected = true;
                this.SelectedNode = data;
            }
        }

        public ICommand ChartDoubleClickCommand { get; private set; }
        public void ChartDoubleClickExecute()
        {
            foreach (var data in this.NodeProfilingData)
            {
                data.Selected = false;
            }

            this.SelectedNode = null;
        }

        public ICommand SelectionChangedCommand { get; private set; }
        public void SelectionChangedExecute(IList obj)
        {
            var selected = obj.OfType<ProfilingDataPoint>().ToDictionary(data => data.Instance.Id);

            foreach (var item in this.NodeProfilingData)
            {
                item.Selected = selected.ContainsKey(item.Instance.Id);
            }

        }

        #endregion
    }
}