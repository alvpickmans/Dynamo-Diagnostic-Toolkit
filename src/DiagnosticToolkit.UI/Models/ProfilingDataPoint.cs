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
        }

        public event Action PointChanged;
        private void OnPointChanged() => PointChanged?.Invoke();

        public void UpdateWeight()
        {
            this.Weight = this.Instance.ExecutionTime.TotalMilliseconds;
            this.OnPointChanged();
        }

        private void OnPositionChanged(IProfilingData data)
        {
            this.X = data.X;
            this.Y = data.Y;

            this.OnPointChanged();
        }

    }
}
