using System.Windows;
using MahApps.Metro.Controls;
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Defaults;
using System.Linq;
using System.Windows.Threading;

namespace DiagnosticToolkit
{
    /// <summary>
    /// Interaction logic for DiagnosticToolkitWindow.xaml
    /// </summary>
    public partial class DiagnosticToolkitWindow
    {
        public DiagnosticToolkitWindow()
        {
            InitializeComponent();

            
            DataContext = this;
        }
    }
}
