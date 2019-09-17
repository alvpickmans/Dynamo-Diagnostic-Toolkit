using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Animation;
using LiveCharts.Configurations;
using LiveCharts.Maps;

namespace DiagnosticToolkit.UI.Models
{
    public static class ChartMappers
    {
        private static Brush NotExecutedBrush = new SolidColorBrush(Color.FromArgb(175, 175, 175, 175));
        private static Brush ExecutionRequestedBrush = new SolidColorBrush(Color.FromArgb(175, 255, 165, 0));
        private static Brush SelectedBrush = new SolidColorBrush(Color.FromArgb(175, 255, 0, 0));

        private static Brush GetFillBrush(this ProfilingDataPoint point)
        {
            if (point.Selected)
                return SelectedBrush;

            if (point.Instance.HasExecutionPending)
                return ExecutionRequestedBrush;

            if (!point.Instance.Executed)
                return NotExecutedBrush;

            return null;
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
            .Fill(value => value.GetFillBrush());
    }
}
