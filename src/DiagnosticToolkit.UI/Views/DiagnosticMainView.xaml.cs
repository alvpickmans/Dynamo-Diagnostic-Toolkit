using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DiagnosticToolkit.UI.ViewModels;
using MahApps.Metro.Controls;

namespace DiagnosticToolkit.UI.Views
{
    /// <summary>
    /// Interaction logic for DiagnosticMainView.xaml
    /// </summary>
    public partial class DiagnosticMainView : MetroWindow
    {
        DiagnosticMainViewModel viewModel;
        public DiagnosticMainView(DiagnosticMainViewModel viewModel)
        {
            InitializeComponent();
            this.viewModel = viewModel;
            this.DataContext = this.viewModel;

            this.Closed += (sender, arg) =>
            {
                this.viewModel.Dispose();
            };
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
