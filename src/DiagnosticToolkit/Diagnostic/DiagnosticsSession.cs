using System;
using System.Collections.Generic;
using System.Linq;
using Dynamo.Core;
using Dynamo.Engine.CodeGeneration;
using Dynamo.Events;
using Dynamo.Graph.Nodes;
using Dynamo.Graph.Workspaces;
using ProtoCore.AST.AssociativeAST;
using VMDataBridge;

namespace DiagnosticToolkit
{
    /// <summary>
    /// This class manages a diagnostic session and the data collected in a session.
    /// </summary>
    class DiagnosticsSession : NotificationObject, IDisposable
    {
        private IWorkspaceModel workspace;
        private Dictionary<Guid, NodeData> data = new Dictionary<Guid, NodeData>();
        private PerformanceStatistics statistics;
        private DateTime executionStartTime;
        private TimeSpan executionTime;

        public DiagnosticsSession(IWorkspaceModel workspace, PerformanceStatistics statistics)
        {
            this.statistics = statistics;
            this.workspace = workspace;
            RegisterEventHandlers(workspace);
            EvaluatedNodes = new List<NodeData>();
        }

        public IWorkspaceModel WorkSpace { get { return workspace; } }

        public string GetTraceData(NodeModel node)
        {
            NodeData nodeData;
            if (data.TryGetValue(node.GUID, out nodeData))
                return nodeData.TraceData;
            return string.Empty;
        }

        public void Dispose()
        {
            UnregisterEventHandlers();
            foreach (var item in data)
            {
                item.Value.Dispose();
            }
            data.Clear();
            EvaluatedNodes.Clear();
        }

        public NodeData GetNodeDataFromGuid(Guid id)
        {
            NodeData nodeData;
            if (data.TryGetValue(id, out nodeData))
                return nodeData;

            var model = workspace.Nodes.FirstOrDefault(n => n.GUID == id);
            if (model != null)
            {
                nodeData = new NodeData(model);
                data.Add(id, nodeData);
            }

            return nodeData;
        }

        void OnNodeAdded(NodeModel obj)
        {
            data.Add(obj.GUID, new NodeData(obj));
        }

        void OnNodeRemoved(NodeModel obj)
        {
            NodeData nodeData;
            if (data.TryGetValue(obj.GUID, out nodeData))
                nodeData.Dispose();

            data.Remove(obj.GUID);
        }

        void OnGraphPreExecution(Dynamo.Session.IExecutionSession session)
        {
            //Reset the node data
            foreach (var item in data)
            {
                item.Value.Reset();
            }

            executionStartTime = DateTime.Now;
        }

        void OnGraphPostExecution(Dynamo.Session.IExecutionSession session)
        {
            executionTime = DateTime.Now.Subtract(executionStartTime);
            DiagnosticsSession.TotalExecutionTime = this.ExecutionTime;

            DiagnosticsSession.MaxExecutionTime = 1;
            EvaluatedNodes.Clear();
            foreach (var item in data)
            {
                if (item.Value.HasPerformanceData)
                {
                    if (item.Value.ExecutionTime > MaxExecutionTime)
                        MaxExecutionTime = item.Value.ExecutionTime;

                    EvaluatedNodes.Add(item.Value);
                    statistics.AddPerformanceData(item.Value.Node, item.Value.GetPerformanceData());
                }
            }
            RaisePropertyChanged("EvaluatedNodes");

            OnSessionExecuted(null);
        }
        private void RegisterEventHandlers(IWorkspaceModel workspace)
        {
            workspace.NodeAdded += OnNodeAdded;
            workspace.NodeRemoved += OnNodeRemoved;
            ExecutionEvents.GraphPreExecution += OnGraphPreExecution;
            ExecutionEvents.GraphPostExecution += OnGraphPostExecution;
        }

        private void UnregisterEventHandlers()
        {
            workspace.NodeAdded -= OnNodeAdded;
            workspace.NodeRemoved -= OnNodeRemoved;
            ExecutionEvents.GraphPreExecution -= OnGraphPreExecution;
            ExecutionEvents.GraphPostExecution -= OnGraphPostExecution;
        }

        public List<NodeData> EvaluatedNodes { get; private set; }

        public int ExecutionTime { get { return (int)executionTime.TotalMilliseconds; } }

        internal static int TotalExecutionTime { get; private set; }

        internal static int MaxExecutionTime { get; private set; }

        #region events

        protected virtual void OnSessionExecuted(EventArgs e)
        {
            EventHandler handler = SessionExecuted;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler SessionExecuted;

        #endregion
    }
}
