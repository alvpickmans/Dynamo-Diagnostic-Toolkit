using DiagnosticToolkit.Core.Interfaces;
using DiagnosticToolkit.Dynamo.Profiling;
using Dynamo.Engine;
using Dynamo.Events;
using Dynamo.Graph.Nodes;
using Dynamo.Graph.Workspaces;
using Dynamo.Models;
using Dynamo.Session;
using Dynamo.ViewModels;
using Dynamo.Wpf.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace DiagnosticToolkit.Dynamo
{
    public class DynamoProfilingManager : IProfilingManager<Session, NodeProfilingData>, IDisposable
    {
        private ViewLoadedParams loadedParameters { get; set; }
        private DynamoViewModel dynamoVM { get; set; }
        private EngineController engineController { get; set; }
        public Session CurrentSession { get; private set; }
        public bool IsEnabled { get; private set; }

        public DynamoProfilingManager(ViewLoadedParams parameters, bool enableProfiling = false)
        {
            this.loadedParameters = parameters;
            this.dynamoVM = parameters.DynamoWindow.DataContext as DynamoViewModel;
            this.engineController = this.dynamoVM.EngineController;

            this.CurrentSession = new Session(parameters.CurrentWorkspaceModel);
            this.RegisterEventHandlers();

            if (enableProfiling)
                this.EnableProfiling();
            else
                this.DisableProfiling();
        }

        private void RegisterEventHandlers()
        {
            this.loadedParameters.CurrentWorkspaceChanged += this.OnWorkspaceChanged;
            ExecutionEvents.GraphPreExecution += OnGraphPreExecution;
            ExecutionEvents.GraphPostExecution += OnGraphPostExecution;
        }

        private void OnWorkspaceChanged(IWorkspaceModel workspace)
        {
            if (this.CurrentSession == null || !this.CurrentSession.Workspace.Equals(workspace))
            {
                this.CurrentSession.Dispose();
                this.CurrentSession = new Session(workspace);
            }
        }

        private void UnregisterEventHandlers()
        {
            ExecutionEvents.GraphPreExecution -= OnGraphPreExecution;
            ExecutionEvents.GraphPostExecution -= OnGraphPostExecution;
        }

        private void OnGraphPreExecution(IExecutionSession session)
        {
            this.CurrentSession?.Start();
        }

        private void OnGraphPostExecution(IExecutionSession session)
        {
            this.CurrentSession.End();
        }

        public void Dispose()
        {
            this.UnregisterEventHandlers();

            this.CurrentSession?.Dispose();
        }

        public void EnableProfiling()
        {
            if (this.IsEnabled)
                return;

            HomeWorkspaceModel workspace = this.CurrentSession != null
                ? this.CurrentSession.Workspace as HomeWorkspaceModel
                : this.loadedParameters.CurrentWorkspaceModel as HomeWorkspaceModel;

            this.engineController.EnableProfiling(true, workspace, workspace.Nodes);
            this.IsEnabled = true;
        }

        public void DisableProfiling()
        {
            if (!this.IsEnabled)
                return;

            HomeWorkspaceModel workspace = this.loadedParameters.CurrentWorkspaceModel as HomeWorkspaceModel;
            this.engineController.EnableProfiling(false, workspace, new List<NodeModel>());
            this.IsEnabled = false;
        }
    }
}