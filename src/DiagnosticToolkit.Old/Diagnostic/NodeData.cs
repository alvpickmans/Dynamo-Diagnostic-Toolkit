using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dynamo.Core;
using Dynamo.Graph.Nodes;
using VMDataBridge;
using LiveCharts.Defaults;
using DiagnosticToolkit.Utilities;

namespace DiagnosticToolkit
{
    class NodeData : ScatterPoint, IDisposable
    {
        public static double MinWeight = 0;
        public static double MaxWeight = 50;

        #region Private Properties
        private TimeSpan? executionTime;
        private DateTime? executionStartTime;
        #endregion

        #region Public Properties
        public NodeModel Node { get; private set; }

        public string NodeId { get => this.Node.GUID.ToString(); }

        //public double CenterX { get => this.Node.CenterX; }

        //public double CenterY { get => this.Node.CenterY; }
        
        public int InputDataSize { get; private set; }

        public int OutputDataSize { get; private set; }

        public bool HasPerformanceData { get => executionTime.HasValue; }
        
        public int ExecutionTime
        {
            get => executionTime.HasValue ? (int)executionTime.Value.TotalMilliseconds : -1;            
        }
        
        public IEnumerable<int> OutputPortsDataSize { get; set; } 

        #endregion

        #region Public Constructors
        public NodeData(NodeModel node)
        {
            Node = node;
            this.X = node.CenterX;
            this.Y = -node.CenterY;
            this.Weight = 0;
            Node.PropertyChanged += OnNodePropertyChanged;
            Node.NodeExecuted += OnNodeExecuted;
        } 
        #endregion

        private void OnNodeExecuted(object sender, NodeExecutedEventArgs e)
        {
            var size = Count(e.Data);
            if (size > 0 && Node.IsInputNode && !executionStartTime.HasValue) //For input node start notification may not come
                executionStartTime = DateTime.Now;

            if (e.Type == NodeExecutedType.Start)
            {
                executionStartTime = DateTime.Now;
                executionTime = null;
                Weight = 0;
                InputDataSize = size;
            }
            else
            {
                executionTime = DateTime.Now.Subtract(executionStartTime.Value);
                Weight = executionTime.Value.Milliseconds;
                executionStartTime = null;
                OutputDataSize = size;
                OutputPortsDataSize = (e.Data as IEnumerable).Cast<object>().Select(Count);
            }
        }

        void OnNodePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "Position")
            {
                this.X = this.Node.CenterX;
                this.Y = -this.Node.CenterY;
            }
                
        }

        public void Reset()
        {
            executionStartTime = null;
            executionTime = null;
            this.Weight = 0;
        }

        public PerformanceData GetPerformanceData()
        {
            return new PerformanceData { ExecutionTime = ExecutionTime, InputSize = InputDataSize, OutputSize = OutputDataSize };
        }


        public void Dispose()
        {
            Node.NodeExecuted -= OnNodeExecuted;
            Node = null;
        }

        private int Count(object data)
        {
            var collection = data as IEnumerable;
            if (collection == null) return 1;

            var count = 0;
            foreach (var item in collection)
            {
                count += Count(item);
            }

            return count;
        }
        
    }
}
