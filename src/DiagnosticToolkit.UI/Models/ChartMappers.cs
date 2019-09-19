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
        private static Brush NotExecutedBrush = new SolidColorBrush(Color.FromArgb(240, 175, 175, 175));
        private static Brush ExecutionRequestedBrush = new SolidColorBrush(Color.FromArgb(240, 255, 165, 0));
        private static RadialGradientBrush SelectedBrush = new RadialGradientBrush(Color.FromArgb(0, 255, 255, 255), Color.FromArgb(240, 255, 255, 255));


        static ChartMappers()
        {
            SelectedBrush.GradientStops.First().BeginAnimation(GradientStop.ColorProperty, RadialGradiantColorAnimation(SelectedBrush, false));
            SelectedBrush.GradientStops.Last().BeginAnimation(GradientStop.ColorProperty, RadialGradiantColorAnimation(SelectedBrush, true));
        }

        private static ColorAnimation RadialGradiantColorAnimation(RadialGradientBrush brush, bool reverse)
        {
            var firstColor = brush.GradientStops.First().Color;
            var lastColor = brush.GradientStops.Last().Color;

            return new ColorAnimation()
            {
                From = reverse ? lastColor : firstColor,
                To = reverse ? firstColor : lastColor,
                Duration = new System.Windows.Duration(TimeSpan.FromSeconds(1)),
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };
        }

        private static Brush GetFillBrush(this ProfilingDataPoint point)
        {
            if (point.Instance.HasExecutionPending)
                return ExecutionRequestedBrush;

            if (!point.Instance.Executed)
                return NotExecutedBrush;

            return null;
        }

        private static Brush GetStrokeBrush(this ProfilingDataPoint point)
        {
            return !point.Selected ? Brushes.Transparent : (Brush)SelectedBrush;
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
