using Dynamo.ViewModels;
using Dynamo.Wpf.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DiagnosticToolkit.Dynamo
{
    public class DiagnosticDynamoManager : IDisposable
    {
        private readonly Version MINIMUM_VERSION = new Version("2.3.0.5885");
        private ViewLoadedParams loadedParameters { get; set; }
        private DynamoViewModel dynamoVM { get; set; }
        private Version dynamoVersion { get; set; }
        private MenuItem mainMenu { get; set; }

        public bool CanUseDiagnostic => dynamoVersion >= MINIMUM_VERSION;

        public DiagnosticDynamoManager(ViewLoadedParams parameters)
        {
            this.loadedParameters = parameters;
            this.dynamoVM = parameters.DynamoWindow.DataContext as DynamoViewModel;
            this.dynamoVersion = parameters.StartupParams.DynamoVersion;

            this.InitializeMenu();
        }

        public void InitializeMenu()
        {

            this.mainMenu = new MenuItem()
            {
                Header = "Diagnostic Toolkic"
            };

            var launchToolkit = new MenuItem()
            {
                Header = "Launch",
            };

            launchToolkit.Click += (sender, args) =>
            {
                if (!this.CanUseDiagnostic)
                {
                    MessageBox.Show($"The Diagnostic Toolkit cannot be used in your current Dynamo Version {dynamoVersion.ToString()}. Minimum version required is {MINIMUM_VERSION.ToString()}.");
                    return;
                }

                MessageBox.Show("Can use disgnostic");
            };

            this.mainMenu.Items.Add(launchToolkit);
            this.loadedParameters.dynamoMenu.Items.Add(this.mainMenu);
        }

        public void Dispose()
        {
            
        }
    }
}
