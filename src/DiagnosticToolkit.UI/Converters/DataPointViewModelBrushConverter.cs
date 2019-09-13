using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DiagnosticToolkit.UI.Converters
{
    /// <summary>
    /// Converter from LiveCharts.Wpf.DataPointViewModel to a Brush color
    /// </summary>
    [ValueConversion(typeof(DataPointViewModel), typeof(Brush))]
    public class DataPointViewModelBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DataPointViewModel point = value as DataPointViewModel;

            if (point == null)
                return null;

            return point.ChartPoint.Fill ?? point.Series.Fill;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
