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
        private ViewLoadedParams viewLoadedParams;
        private MenuItem sampleMenuItem;
        private DynamoViewModel vm;
        private DynamoModel model;

        DiagnosticToolkitWindowViewModel diagnosticViewModel { get; set; }


        public void Dispose()
        {
        }

        public void Startup(ViewStartupParams p)
        {

        }

        public void Loaded(ViewLoadedParams p)
        {
            // Save a reference to your loaded parameters.
            // You'll need these later when you want to use
            // the supplied workspaces
            viewLoadedParams = p;
            vm = p.DynamoWindow.DataContext as DynamoViewModel;
            model = vm.Model;

            sampleMenuItem = new MenuItem { Header = "DynaNostic" };
            sampleMenuItem.Click += (sender, args) =>
            {
                diagnosticViewModel = new DiagnosticToolkitWindowViewModel(p, model);
                var window = new DiagnosticToolkitWindow
                {
                    DataContext = diagnosticViewModel,
                    // Set the owner of the window to the Dynamo window.
                    Owner = p.DynamoWindow
                };

                diagnosticViewModel.AssignWindow(window);

                window.Left = window.Owner.Left + 400;
                window.Top = window.Owner.Top + 200;

                // Show a modeless window.
                window.Show();
            };
            p.AddMenuItem(MenuBarType.View, sampleMenuItem);
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