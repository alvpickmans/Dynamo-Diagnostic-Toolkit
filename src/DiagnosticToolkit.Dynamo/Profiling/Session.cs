using DiagnosticToolkit.Core.Interfaces;
using Dynamo.Engine;
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
    public class Session : IProfilingSession<NodeProfilingData>, IDisposable
    {
        public Guid Guid { get; private set; }
        public TimeSpan ExecutionTime { get; private set; }
        public string Name => Workspace?.Name;
        public IEnumerable<NodeProfilingData> ProfilingData => nodesData?.Values;
        public IWorkspaceModel Workspace { get; private set; }
        private Dictionary<Guid, NodeProfilingData> nodesData;
        private DateTime startTime;

        /// <summary>
        /// Creates a new instance of Dynamo Profiling Session
        /// </summary>
        /// <param name="workspace"></param>
        public Session(IWorkspaceModel workspace)
        {
            this.Guid = Guid.NewGuid();
            this.Workspace = workspace;
            this.nodesData = CollectNodeData(workspace);
            this.RegisterEventHandlers();
        }

        /// <summary>
        /// Starts the profiling session.
        /// </summary>
        /// <returns></returns>
        public Session Start()
        {            
            this.startTime = DateTime.Now;
            return this;
        }

        /// <summary>
        /// Ends the profiling session.
        /// </summary>
        /// <returns></returns>
        public Session End()
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
        }

        private void UnregisterEventHandlers()
        {
            this.Workspace.NodeAdded -= OnNodeAdded;
            this.Workspace.NodeRemoved -= OnNodeRemoved;
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

        public void Dispose()
        {
            UnregisterEventHandlers();
        }

        #region Events
        public event EventHandler SessionStarted;
        protected void OnSessionStarted(EventArgs e) => SessionStarted?.Invoke(this, e);

        public event EventHandler SessionEnded;
        protected void OnSessionEnded(EventArgs e) => SessionEnded?.Invoke(this, e);

        public event EventHandler DataAdded;
        protected void OnDataAdded(EventArgs e) => DataAdded?.Invoke(this, e);

        public event EventHandler DataRemoved;
        protected void OnDataRemoved(EventArgs e) => DataRemoved?.Invoke(this, e);
        #endregion
    }
}
