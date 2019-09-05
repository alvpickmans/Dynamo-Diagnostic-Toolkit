using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveCharts.Configurations;
using LiveCharts.Maps;

namespace DiagnosticToolkit.UI.Models
{
    public static class ChartMappers
    {
        public static WeightedMapper<ProfilingDataPoint> ProfilingDataPointMapper = Mappers
            .Weighted<ProfilingDataPoint>()
            .X(value => value.X)
            .Y(value => value.Y)
            .Weight(value => value.Weight);
    }
}
