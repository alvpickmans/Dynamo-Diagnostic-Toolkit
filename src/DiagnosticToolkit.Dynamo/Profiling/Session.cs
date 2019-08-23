using DiagnosticToolkit.Core.Interfaces;
using Dynamo.Events;
using Dynamo.Graph.Nodes;
using Dynamo.Graph.Workspaces;
using Dynamo.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiagnosticToolkit.Dynamo.Profiling
{
    /// <summary>
    /// Profiling session for a Dynamo Graph.
    /// </summary>
    public class Session : IProfilingSession, IDisposable
    {
        public TimeSpan ExecutionTime { get; private set; }
        public string Name => Workspace?.Name;
        public IWorkspaceModel Workspace { get; private set; }

        private Dictionary<Guid, NodeProfilingData> nodesData;
        private DateTime startTime;

        /// <summary>
        /// Creates a new instance of Dynamo Profiling Session
        /// </summary>
        /// <param name="workspace"></param>
        public Session(IWorkspaceModel workspace)
        {
            this.Workspace = workspace;
            this.nodesData = CollectNodeData(workspace);
            this.RegisterEventHandlers();
        }

        private Session StartSession()
        {
            this.startTime = DateTime.Now;
            return this;
        }

        private Session EndSession()
        {
            this.ExecutionTime = DateTime.Now.Subtract(this.startTime);

            return this;
        }

        private static Dictionary<Guid, NodeProfilingData> CollectNodeData(IWorkspaceModel workspace)
        {
            return workspace.Nodes
                .ToDictionary(node => node.GUID, node => new NodeProfilingData(node));
        }

        private void RegisterEventHandlers()
        {
            this.Workspace.NodeAdded += OnNodeAdded;
            this.Workspace.NodeRemoved += OnNodeRemoved;
            ExecutionEvents.GraphPreExecution += OnGraphPreExecution;
            ExecutionEvents.GraphPostExecution += OnGraphPostExecution;
        }

        private void UnregisterEventHandlers()
        {
            this.Workspace.NodeAdded -= OnNodeAdded;
            this.Workspace.NodeRemoved -= OnNodeRemoved;
            ExecutionEvents.GraphPreExecution -= OnGraphPreExecution;
            ExecutionEvents.GraphPostExecution -= OnGraphPostExecution;
        }

        private void OnNodeAdded(NodeModel node)
        {
            nodesData.Add(node.GUID, new NodeProfilingData(node));
            OnDataAdded(null);
        }

        private void OnNodeRemoved(NodeModel node)
        {
            if (!nodesData.TryGetValue(node.GUID, out NodeProfilingData data))
                return;

            data.Dispose();
            nodesData.Remove(node.GUID);
            this.OnDataRemoved(null);
        }

        private void OnGraphPreExecution(IExecutionSession session)
        {
            this.StartSession();
        }

        private void OnGraphPostExecution(IExecutionSession session)
        {
            this.EndSession();
        }

        public void Dispose()
        {
            UnregisterEventHandlers();
        }

        public event EventHandler SessionStarted;
        public event EventHandler SessionEnded;

        public event EventHandler DataAdded;
        protected void OnDataAdded(EventArgs e)
        {
            DataAdded?.Invoke(this, e);
        }

        public event EventHandler DataRemoved;
        protected void OnDataRemoved(EventArgs e)
        {
            DataRemoved?.Invoke(this, e);
        }
    }
}
