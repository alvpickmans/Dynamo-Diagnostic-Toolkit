using DiagnosticToolkit.Core.Interfaces;
using LiveCharts;
using LiveCharts.Defaults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyChanged;

namespace DiagnosticToolkit.UI.Models
{
    public class ProfilingDataPoint : ScatterPoint, IObservableChartPoint
    {
        [DoNotCheckEquality]
        public IProfilingData Instance { get; private set; }

        public ProfilingDataPoint(IProfilingData profilingData) : base(profilingData.X, profilingData.Y)
        {
            this.Instance = profilingData;
            this.Instance.PositionChanged += this.OnPositionChanged;
            this.Instance.Modified += this.OnModified;
            this.Weight = Math.Round(this.Instance.ExecutionTime.TotalMilliseconds);
        }

        public void ForceExecution()
        {
            if (this.Instance.CanRequestExecution)
                this.Instance.RequestExecution();

            this.OnPointChanged();
        }

        public void UpdateWeight()
        {
            this.Weight = Math.Round(this.Instance.ExecutionTime.TotalMilliseconds);
            this.OnPointChanged();
        }

        private void OnModified(IProfilingData obj)
        {
            this.OnPointChanged();
            // Hack to raise property changed
            this.Instance = this.Instance;
        }

        public event Action PointChanged;
        private void OnPointChanged() => PointChanged?.Invoke();

        private void OnPositionChanged(IProfilingData data)
        {
            this.X = data.X;
            this.Y = data.Y;

            this.OnPointChanged();
        }

    }
}
