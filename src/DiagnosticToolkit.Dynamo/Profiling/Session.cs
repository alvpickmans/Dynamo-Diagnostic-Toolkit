using DiagnosticToolkit.Core.Interfaces;
using Dynamo.Graph.Nodes;
using Dynamo.Graph.Workspaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiagnosticToolkit.Dynamo.Profiling
{
    /// <summary>
    /// Profiling session for a Dynamo Graph.
    /// </summary>
    public class Session : IProfilingSession, IDisposable
    {
        public Guid Guid { get; private set; }
        public TimeSpan ExecutionTime { get; private set; }
        public string Name => Workspace?.Name;

        private Dictionary<Guid, NodeProfilingData> nodesData;
        public IEnumerable<IProfilingData> ProfilingData => nodesData?.Values;

        public IWorkspaceModel Workspace { get; private set; }
        private DateTime? startTime;
        public bool Executing => startTime.HasValue;

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
            foreach (var data in nodesData.Values)
            {
                data.Reset();
            }

            this.startTime = DateTime.Now;

            this.OnSessionStarted(EventArgs.Empty);
            return this;
        }

        /// <summary>
        /// Ends the profiling session.
        /// </summary>
        /// <returns></returns>
        public Session End()
        {
            if (this.Executing)
            {
                this.ExecutionTime = DateTime.Now.Subtract(this.startTime.Value);
                this.startTime = null;
                this.OnSessionEnded(EventArgs.Empty);
            }

            return this;
        }

        public void Clear()
        {
            this.nodesData.Clear();

            this.OnSessionCleared(EventArgs.Empty);
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
            var data = new NodeProfilingData(node);
            nodesData.Add(node.GUID, data);
            this.OnDataAdded(data);
        }

        private void OnNodeRemoved(NodeModel node)
        {
            if (!nodesData.TryGetValue(node.GUID, out NodeProfilingData data))
                return;

            data.Dispose();
            nodesData.Remove(node.GUID);
            this.OnDataRemoved(data);
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

        public event EventHandler SessionCleared;

        protected void OnSessionCleared(EventArgs e) => SessionCleared?.Invoke(this, e);

        public event Action<IProfilingData> DataAdded;

        protected void OnDataAdded(IProfilingData data) => DataAdded?.Invoke(data);

        public event Action<IProfilingData> DataRemoved;

        protected void OnDataRemoved(IProfilingData data) => DataRemoved?.Invoke(data);

        #endregion Events
    }
}