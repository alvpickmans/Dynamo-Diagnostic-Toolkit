using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using LiveCharts.Configurations;
using LiveCharts.Maps;

namespace DiagnosticToolkit.UI.Models
{
    public static class ChartMappers
    {
        private static Brush NotExecutedBrush = new SolidColorBrush(Color.FromArgb(200, 175, 175, 175));
        private static Brush ExecutionRequestedBrush = new SolidColorBrush(Color.FromArgb(200, 255, 165, 0));

        private static Brush GetFillBrush(this ProfilingDataPoint point)
        {
            return point.Instance.Executed ? NotExecutedBrush : null;
        }

        private static Brush GetStrokeBrush(this ProfilingDataPoint point)
        {
            return point.Instance.HasExecutionPending ? ExecutionRequestedBrush : point.GetFillBrush();
        }

        public static WeightedMapper<ProfilingDataPoint> ProfilingDataPointMapper = Mappers
            .Weighted<ProfilingDataPoint>()
            .X(value => value.X)
            .Y(value => value.Y)
            .Weight(value => value.Weight)
            .Fill(value => value.GetFillBrush())
            .Stroke(value => value.GetStrokeBrush());
    }
}
