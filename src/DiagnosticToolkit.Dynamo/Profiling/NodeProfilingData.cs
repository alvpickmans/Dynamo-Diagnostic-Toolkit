using DiagnosticToolkit.Core.Interfaces;
using Dynamo.Graph.Nodes;
using Dynamo.Utilities;
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

        public double X { get; private set; }
        public double Y { get; private set; }

        private DateTime startTime { get; set; }

        public NodeModel Node { get; private set; }
        public string NodeId => this.Node?.GUID.ToString();

        public NodeProfilingData(NodeModel node)
        {
            this.Node = node;
            this.RegisterEvents();
        }

        private void UpdatePosition(Point2D position)
        {
            this.X = position.X;
            // Dynamo considers the Y axis positive to be from top to bottom
            this.Y = position.Y * -1;
        }

        private void RegisterEvents()
        {
            if (this.Node == null)
                return;

            this.Node.NodeExecutionBegin += this.OnNodeExecutionBegin;
            this.Node.NodeExecutionEnd += this.OnNodeExecutionEnd;
            this.Node.PropertyChanged += this.OnNodePropertyChanged;
        }

        private void UnregisterEvents()
        {
            if (this.Node == null)
                return;

            this.Node.NodeExecutionBegin -= this.OnNodeExecutionBegin;
            this.Node.NodeExecutionEnd -= this.OnNodeExecutionEnd;
        }

        private void OnNodeExecutionBegin(NodeModel obj)
        {
            this.startTime = DateTime.Now;
        }

        private void OnNodeExecutionEnd(NodeModel obj)
        {
            this.ExecutionTime = DateTime.Now.Subtract(this.startTime);
        }

        private void OnNodePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(NodeModel.Position))
                return;

            this.UpdatePosition(this.Node.Position);
        }

        public void Dispose()
        {
            this.UnregisterEvents();
        }
    }
}
