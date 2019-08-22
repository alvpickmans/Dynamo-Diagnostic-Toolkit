using Dynamo.ViewModels;
using Dynamo.Wpf.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DiagnosticToolkit.UI;
using DiagnosticToolkit.UI.Views;

namespace DiagnosticToolkit.Dynamo
{
    public class DynamoProfilingManager : IDisposable
    {
        private ViewLoadedParams loadedParameters { get; set; }
        private DynamoViewModel dynamoVM { get; set; }
        private MenuItem mainMenu { get; set; }

        public DynamoProfilingManager(ViewLoadedParams parameters)
        {
            this.loadedParameters = parameters;
            this.dynamoVM = parameters.DynamoWindow.DataContext as DynamoViewModel;

            this.InitializeMenu();
        }

        public void InitializeMenu()
        {

            this.mainMenu = new MenuItem()
            {
                Header = DiagnosticViewExtension.NAME
            };

            var launchToolkit = new MenuItem()
            {
                Header = "Launch",
            };

            launchToolkit.Click += (sender, args) =>
            {
                DiagnosticMainView view = new DiagnosticMainView();
                view.Show();
            };

            this.mainMenu.Items.Add(launchToolkit);
            this.loadedParameters.dynamoMenu.Items.Add(this.mainMenu);
        }

        public void Dispose()
        {
            
        }
    }
}
