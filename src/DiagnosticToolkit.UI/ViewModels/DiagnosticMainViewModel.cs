using DiagnosticToolkit.Core.Interfaces;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
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

        public SeriesCollection NodeProfilingData { get; private set; }

        public DiagnosticMainViewModel(IProfilingManager manager)
        {
            this.manager = manager ?? throw new ArgumentNullException(nameof(manager));
            this.session = this.manager.CurrentSession;

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
            this.RegisterSessionEvents(this.session);
        }

        private void UnregisterEvents()
        {
            this.manager.SessionChanged -= this.OnSessionChanged;
            this.UnregisterSessionEvents(this.session);
        }

        private void RegisterSessionEvents(IProfilingSession session)
        {
            if (this.session == null)
                return;

            this.session.SessionStarted += this.OnSessionStarted;
            this.session.SessionEnded += this.OnSessionEnded;
            this.session.DataAdded += this.OnDataAdded;
        }

        private void OnDataAdded(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void UnregisterSessionEvents(IProfilingSession session)
        {
            if (this.session == null)
                return;

            this.session.SessionStarted -= this.OnSessionStarted;
            this.session.SessionEnded -= this.OnSessionEnded;
        }

        private void OnSessionChanged(IProfilingSession session)
        {
            this.UnregisterSessionEvents(this.session);

            this.session = session;

            this.RegisterSessionEvents(this.session);
        }

        private void OnSessionEnded(object sender, EventArgs e)
        {
            var scatter = this.session.ProfilingData.Select(data => new ScatterPoint(data.X, data.Y, data.ExecutionTime.TotalMilliseconds));

            double MinDiameter = 1;
            double MaxDiameter = 50;

            CallingDispatcher.Invoke(() =>
            {
                this.NodeProfilingData = new SeriesCollection()
                {
                    new ScatterSeries
                    {
                        Values = new ChartValues<ScatterPoint>(scatter),
                        MinPointShapeDiameter = MinDiameter,
                        MaxPointShapeDiameter = MaxDiameter,
                    },
                };
            });
        }

        private void OnSessionStarted(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        #endregion Events
    }
}