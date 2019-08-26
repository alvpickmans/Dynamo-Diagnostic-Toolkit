using DiagnosticToolkit.Core.Interfaces;
using Dynamo.Graph.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiagnosticToolkit.Dynamo.Profiling
{
    public class NodeProfilingData : IProfilingData, IDisposable
    {
        public TimeSpan ExecutionTime { get; private set; }

        private DateTime startTime { get; set; }

        public NodeModel Node { get; private set; }
        public string NodeId => this.Node?.GUID.ToString();

        public NodeProfilingData(NodeModel node)
        {
            this.Node = node;
            this.RegisterEvents();
        }

        private void RegisterEvents()
        {
            if (this.Node == null)
                return;

            this.Node.NodeExecutionBegin += this.OnNodeExecutionBegin;
            this.Node.NodeExecutionEnd += this.OnNodeExecutoinEnd;
        }

        private void UnregisterEvents()
        {
            if (this.Node == null)
                return;

            this.Node.NodeExecutionBegin -= this.OnNodeExecutionBegin;
            this.Node.NodeExecutionEnd -= this.OnNodeExecutoinEnd;
        }

        private void OnNodeExecutionBegin(NodeModel obj)
        {
            this.startTime = DateTime.Now;
        }

        private void OnNodeExecutoinEnd(NodeModel obj)
        {
            this.ExecutionTime = DateTime.Now.Subtract(this.startTime);
        }

        public void Dispose()
        {
            this.UnregisterEvents();
        }
    }
}
