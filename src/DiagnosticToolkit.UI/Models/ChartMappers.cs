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
        private static Brush NotExecutedBrush = new SolidColorBrush(Color.FromArgb(255, 100, 100, 100));

        public static WeightedMapper<ProfilingDataPoint> ProfilingDataPointMapper = Mappers
            .Weighted<ProfilingDataPoint>()
            .X(value => value.X)
            .Y(value => value.Y)
            .Weight(value => value.Weight)
            .Fill(value => !value.Instance.Executed ? NotExecutedBrush : null);
    }
}
