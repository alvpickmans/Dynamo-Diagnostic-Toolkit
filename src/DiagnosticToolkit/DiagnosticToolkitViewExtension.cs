using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Dynamo.Wpf.Extensions;
using Dynamo.Engine.CodeGeneration;
using Dynamo.ViewModels;
using Dynamo.Models;
using System.IO;
using Dynamo.Events;
using Dynamo.Graph.Workspaces;

namespace DiagnosticToolkit
{
    /// <summary>
    /// DiagnosticToolkik view extension
    /// </summary>
    public class DiagnosticToolkitViewExtension : IViewExtension
    {
        private MenuItem toolkitMenuItem;
        private DynamoViewModel vm;

        DiagnosticToolkitWindowViewModel diagnosticViewModel { get; set; }


        public void Dispose()
        {
        }

        public void Startup(ViewStartupParams p)
        {

        }

        public void Loaded(ViewLoadedParams parameters)
        {
            // Save a reference to your loaded parameters.
            // You'll need these later when you want to use
            // the supplied workspaces
            vm = parameters.DynamoWindow.DataContext as DynamoViewModel;

            toolkitMenuItem = new MenuItem { Header = "Diagnostic Toolkit" };
            toolkitMenuItem.Click += (sender, args) =>
            {
                diagnosticViewModel = new DiagnosticToolkitWindowViewModel(parameters);
                var window = new DiagnosticToolkitWindow
                {
                    DataContext = diagnosticViewModel,
                    Owner = parameters.DynamoWindow
                };

                diagnosticViewModel.AssignWindow(window);

                window.Left = window.Owner.Left + 400;
                window.Top = window.Owner.Top + 200;

                // Show a modeless window.
                window.Show();
            };

            parameters.AddSeparator(MenuBarType.View, new Separator());
            parameters.AddMenuItem(MenuBarType.View, toolkitMenuItem);
        }

        public void Shutdown()
        {
            diagnosticViewModel.SaveData();
        }

        public string UniqueId
        {
            get
            {
                return Guid.NewGuid().ToString();
            }
        }

        public string Name
        {
            get
            {
                return "DiagnosticToolkit View Extension";
            }
        }

    }
}