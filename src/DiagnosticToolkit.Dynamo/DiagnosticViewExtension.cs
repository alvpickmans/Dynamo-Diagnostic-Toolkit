using Dynamo.Wpf.Extensions;
using System;
using System.Windows;

namespace DiagnosticToolkit.Dynamo
{
    public class DiagnosticViewExtension : IViewExtension
    {
        public string UniqueId => "EF1D3B3E-3991-46BE-B75F-8429A49AE58C";
        public string Name => "Diagnostic Toolkit for Dynamo";

        private DiagnosticDynamoManager manager { get; set; }

        public void Startup(ViewStartupParams parameters)
        {
        }

        public void Loaded(ViewLoadedParams parameters)
        {
            this.manager = new DiagnosticDynamoManager(parameters);
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
