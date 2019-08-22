using Dynamo.Wpf.Extensions;
using System;
using System.Windows;
using System.Windows.Controls;

namespace DiagnosticToolkit.Dynamo
{
    public class DiagnosticViewExtension : IViewExtension
    {
        public const string NAME = "Diagnostic Toolkit";

        private readonly Version MINIMUM_VERSION = new Version("2.3.0.5885");
        private Version currentVersion { get; set; }
        public string UniqueId => "EF1D3B3E-3991-46BE-B75F-8429A49AE58C";
        public string Name => NAME;

        private DynamoProfilingManager manager { get; set; }

        public void Startup(ViewStartupParams parameters)
        {
            this.currentVersion = parameters.DynamoVersion;
        }

        public void Loaded(ViewLoadedParams parameters)
        {
            if(this.currentVersion >= MINIMUM_VERSION)
            {
                this.manager = new DynamoProfilingManager(parameters);
                return;
            }

            var menu = new MenuItem()
            {
                Header = this.Name
            };

            menu.Click += (sender, arg)=>
            {
                MessageBox.Show(
                    $"Diagnostic Toolkit cannot be used on your current Dynamo Version {this.currentVersion}.\nMinimum version required is {MINIMUM_VERSION}",
                    this.Name);
            }

        }

        public void Shutdown()
        {
           // throw new NotImplementedException();
        }

        public void Dispose()
        {
            manager.Dispose();
        }
    }
}
