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
    public class ProfilingDataPoint : ScatterPoint
    {
        public IProfilingData Instance { get; private set; }

        public ProfilingDataPoint(IProfilingData profilingData)
        {
            this.Instance = profilingData;
            this.X = profilingData.X;
            this.Y = profilingData.Y;
            this.Weight = profilingData.ExecutionTime.TotalMilliseconds;

            this.Instance.PositionChanged += this.OnPositionChanged;
        }

        public void UpdateWeight()
        {
            this.Weight = this.Instance.ExecutionTime.TotalMilliseconds;
        }

        private void OnPositionChanged(IProfilingData data)
        {
            this.X = data.X;
            this.Y = data.Y;
        }

    }
}
