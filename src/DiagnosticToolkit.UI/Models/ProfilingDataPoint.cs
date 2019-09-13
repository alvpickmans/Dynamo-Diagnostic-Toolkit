using DiagnosticToolkit.Core.Interfaces;
using LiveCharts;
using LiveCharts.Defaults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiagnosticToolkit.UI.Models
{
    public class ProfilingDataPoint : ScatterPoint, IObservableChartPoint
    {
        public IProfilingData Instance { get; private set; }

        public ProfilingDataPoint(IProfilingData profilingData) : base(profilingData.X, profilingData.Y)
        {
            this.Instance = profilingData;
            this.Instance.PositionChanged += this.OnPositionChanged;
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
