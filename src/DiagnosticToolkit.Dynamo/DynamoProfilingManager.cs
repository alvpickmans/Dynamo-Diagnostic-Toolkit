using DiagnosticToolkit.Dynamo.Profiling;
using Dynamo.Events;
using Dynamo.Graph.Workspaces;
using Dynamo.Models;
using Dynamo.Session;
using Dynamo.ViewModels;
using Dynamo.Wpf.Extensions;
using System;
using System.Windows.Controls;

namespace DiagnosticToolkit.Dynamo
{
    public class DynamoProfilingManager : IDisposable
    {
        private ViewLoadedParams loadedParameters { get; set; }
        private DynamoViewModel dynamoVM { get; set; }
        private DynamoModel dynamoModel { get; set; }
        public Session CurrentSession { get; private set; }

        public DynamoProfilingManager(ViewLoadedParams parameters)
        {
            this.loadedParameters = parameters;
            this.dynamoVM = parameters.DynamoWindow.DataContext as DynamoViewModel;
            this.dynamoModel = this.dynamoVM.Model;

            this.CurrentSession = new Session(parameters.CurrentWorkspaceModel);

            this.RegisterEventHandlers();
        }

        private void RegisterEventHandlers()
        {
            this.dynamoModel.WorkspaceAdded += OnCurrentWorkspaceChanged;
        }

        private void UnregisterEventHandlers()
        {
            this.dynamoModel.WorkspaceAdded -= OnCurrentWorkspaceChanged;
        }

        private void OnCurrentWorkspaceChanged(IWorkspaceModel workspace)
        {
            if (this.CurrentSession != null)
                this.CurrentSession.Dispose();

            this.CurrentSession = new Session(workspace);
        }

        public void Dispose()
        {
            this.UnregisterEventHandlers();
        }
    }
}