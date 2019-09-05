using DiagnosticToolkit.Core.Interfaces;
using DiagnosticToolkit.Dynamo.Profiling;
using Dynamo.Engine;
using Dynamo.Graph.Nodes;
using Dynamo.Graph.Workspaces;
using Dynamo.Models;
using Dynamo.ViewModels;
using Dynamo.Wpf.Extensions;
using System;
using System.Collections.Generic;

namespace DiagnosticToolkit.Dynamo
{
    public class DynamoProfilingManager : IProfilingManager, IDisposable
    {
        private ViewLoadedParams loadedParameters { get; set; }
        private DynamoViewModel dynamoVM { get; set; }
        private EngineController engineController { get; set; }

        private Session dynamoSession { get; set; }
        public IProfilingSession CurrentSession => this.dynamoSession;
        public bool IsEnabled { get; private set; }

        public DynamoProfilingManager(ViewLoadedParams parameters, bool enableProfiling = false)
        {
            this.loadedParameters = parameters;
            this.dynamoVM = parameters.DynamoWindow.DataContext as DynamoViewModel;
            this.engineController = this.dynamoVM.EngineController;

            this.RegisterEventHandlers();
        }

        #region Manager Events

        public event Action<IProfilingSession> SessionChanged;

        private void OnSessionChanged(IProfilingSession session) => this.SessionChanged?.Invoke(session);

        #endregion Manager Events

        private void RegisterEventHandlers()
        {
            this.dynamoVM.Model.WorkspaceHidden += this.OnWorkspaceHidden;
            this.dynamoVM.Model.WorkspaceCleared += this.OnWorkspaceCleared;
            this.loadedParameters.CurrentWorkspaceChanged += OnWorkspaceChanged;
        }

        private void UnregisterEventHandlers()
        {
            this.dynamoVM.Model.WorkspaceHidden -= this.OnWorkspaceHidden;
            this.loadedParameters.CurrentWorkspaceChanged -= OnWorkspaceChanged;
        }

        private void RegisterWorkspaceEvents(IWorkspaceModel workspace)
        {
            if (workspace is HomeWorkspaceModel homeWorkspace)
            {
                homeWorkspace.EvaluationStarted += this.OnEvaluationStarted;
                homeWorkspace.EvaluationCompleted += this.OnEvaluationCompleted;
            }
        }

        private void UnregisterWorkspaceEvents(IWorkspaceModel workspace)
        {
            if (workspace is HomeWorkspaceModel homeWorkspace)
            {
                homeWorkspace.EvaluationStarted -= this.OnEvaluationStarted;
                homeWorkspace.EvaluationCompleted -= this.OnEvaluationCompleted;
            }
        }
        private Session SetCurrentSession(IWorkspaceModel workspace)
        {
            if(this.dynamoSession != null)
            {
                this.UnregisterWorkspaceEvents(this.dynamoSession.Workspace);
                this.dynamoSession.Dispose();
            }

            this.dynamoSession = new Session(workspace);
            this.RegisterWorkspaceEvents(this.dynamoSession.Workspace);

            this.OnSessionChanged(this.CurrentSession);
            return this.dynamoSession;
        }
        private void OnWorkspaceCleared(WorkspaceModel workspace)
        {
            // This happens when a new file is opened from start page
            if (this.dynamoSession == null)
                this.SetCurrentSession(workspace);
            
            // This happens when a file is closed or a new empty file is opened
            else if (this.dynamoSession.Workspace.Equals(workspace))
            {
                // When an saved file is closed, the CurrentWorkspace keeps its Name.
                if (String.IsNullOrWhiteSpace(workspace.FileName))
                    workspace.Name = "Home";

                this.dynamoSession.Clear();
            }
        }

        private void OnWorkspaceHidden(WorkspaceModel workspace)
        {
            if (workspace is HomeWorkspaceModel homeWorkspace)
            {
                homeWorkspace.EvaluationStarted -= OnEvaluationStarted;
                homeWorkspace.EvaluationCompleted -= this.OnEvaluationCompleted;
            }
        }

        private void OnWorkspaceChanged(IWorkspaceModel workspace)
        {
            if (this.dynamoSession != null && this.dynamoSession.Workspace.Equals(workspace))
                return;

            if (this.dynamoSession != null && !this.dynamoSession.Workspace.Equals(workspace))
                this.dynamoSession.Dispose();

            this.SetCurrentSession(workspace);
        }

        private void OnEvaluationStarted(object sender, EventArgs e)
        {
            HomeWorkspaceModel workspace = sender as HomeWorkspaceModel;
            if (workspace == null)
                return;

            if (!this.dynamoSession.Workspace.Equals(workspace))
                this.OnWorkspaceChanged(workspace);

            if (!this.engineController.Equals(workspace.EngineController))
                this.ResetEngineController(workspace.EngineController, this.IsEnabled);

            if (this.IsEnabled)
                this.dynamoSession?.Start();
        }

        private void OnEvaluationCompleted(object sender, EvaluationCompletedEventArgs e)
        {
            if (this.IsEnabled)
                this.dynamoSession.End();
        }

        private void ResetEngineController(EngineController engineController, bool enableProfiling)
        {
            this.DisableProfiling();

            this.engineController = engineController;

            if (enableProfiling)
                this.EnableProfiling();
        }

        public void Dispose()
        {
            this.UnregisterEventHandlers();

            this.dynamoSession?.Dispose();
        }

        public void EnableProfiling()
        {
            if (this.IsEnabled)
                return;

            HomeWorkspaceModel workspace = this.dynamoSession != null
                ? this.dynamoSession.Workspace as HomeWorkspaceModel
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