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
        private DateTime? startTime { get; set; }
        private DateTime? endTime { get; set; }
        public TimeSpan ExecutionTime { get; private set; }
        public bool Executed => this.startTime.HasValue;

        public bool CanRequestExecution => true;
        public bool HasExecutionPending => this.Node.NeedsForceExecution || this.Node.IsModified;

        public NodeModel Node { get; private set; }
        public string NodeId => this.Node?.GUID.ToString();
        public string Name => this.Node.Name;
        public string Id => this.Node.GUID.ToString();
        public double X { get; private set; }
        public double Y { get; private set; }

        public bool Selected
        {
            get => this.Node.IsSelected;
            set
            {
                if (value)
                    this.Node.Select();
                else
                    this.Node.IsSelected = false;
            }
        }


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

            this.OnPositionChanged(this);
        }

        public void RequestExecution()
        {
            RequestDownstreamExecution(this.Node);
        }

        // Based on Dynamo's https://github.com/DynamoDS/Dynamo/blob/34bd5ef798f73fe5cd6d018e0e19889c93f097cb/src/DynamoCore/Graph/Nodes/NodeModel.cs#L1021
        private static void RequestDownstreamExecution(NodeModel node, HashSet<NodeModel> nodes = null)
        {
            if (nodes == null)
                nodes = new HashSet<NodeModel>();

            if (nodes.Contains(node))
                return;

            node.MarkNodeAsModified(true);
            nodes.Add(node);

            var sets = node.OutputNodes.Values;
            var outputNodes = sets.SelectMany(set => set.Select(t => t.Item2));

            foreach (var outputNode in outputNodes)
            {
                RequestDownstreamExecution(outputNode, nodes);
            }
        }

        public void Reset()
        {
            this.startTime = null;
            this.endTime = null;
        }

        #region ProfilingData Events
        public event Action<IProfilingData> Modified;
        private void OnModified(IProfilingData data) => this.Modified?.Invoke(data);

        public event Action<IProfilingData> PositionChanged;
        private void OnPositionChanged(IProfilingData data) => this.PositionChanged?.Invoke(data);

        public event Action<IProfilingData> ProfilingExecuted;
        private void OnProfilingExecuted(IProfilingData data) => this.ProfilingExecuted?.Invoke(data);
        #endregion

        #region Dynamo Events
        private void RegisterEvents()
        {
            if (this.Node == null)
                return;

            this.Node.NodeExecutionBegin += this.OnNodeExecutionBegin;
            this.Node.NodeExecutionEnd += this.OnNodeExecutionEnd;
            this.Node.PropertyChanged += this.OnNodePropertyChanged;
            this.Node.Modified += this.OnNodeModified;
        }


        private void UnregisterEvents()
        {
            if (this.Node == null)
                return;

            this.Node.NodeExecutionBegin -= this.OnNodeExecutionBegin;
            this.Node.NodeExecutionEnd -= this.OnNodeExecutionEnd;
            this.Node.PropertyChanged -= this.OnNodePropertyChanged;
            this.Node.Modified -= this.OnNodeModified;
        }

        private void OnNodeExecutionBegin(NodeModel obj)
        {
            this.startTime = DateTime.Now;
        }

        private void OnNodeExecutionEnd(NodeModel obj)
        {
            // For some reason, Dynamo is calling execution ended more than once for the same node
            // If not for this `endTime` check, it will overwrite Execution time;
            if (!this.startTime.HasValue || this.endTime.HasValue)
                return;

            this.endTime = DateTime.Now;
            this.ExecutionTime = this.endTime.Value.Subtract(this.startTime.Value);
            this.OnProfilingExecuted(this);
        }

        private void OnNodePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

            if (e.PropertyName.Equals(nameof(NodeModel.Position)))
                this.UpdatePosition(this.Node.Position);

            if (e.PropertyName.Equals(nameof(NodeModel.Name)) || e.PropertyName.Equals(nameof(NodeModel.IsSelected)))
                this.OnModified(this);               

        } 

        private void OnNodeModified(NodeModel obj) => this.OnModified(this);
        #endregion

        public void Dispose()
        {
            this.UnregisterEvents();
        }
    }
}
