using Dynamo.Controls;
using Dynamo.Wpf.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dynamo.Graph.Nodes;
using System.Windows;

namespace DiagnosticToolkit.Utilities
{
    public class NodeViewCollector
    {
        private Window dynamoWindow;
        private Guid lastGuidAdded { get; set; }
        private Dictionary<Guid, NodeView> collector = new Dictionary<Guid, NodeView>();

        public NodeViewCollector(ViewLoadedParams parameters)
        {
            dynamoWindow = parameters.DynamoWindow;
            parameters.CurrentWorkspaceModel.NodeAdded += OnNodeAdded;
            parameters.CurrentWorkspaceModel.NodeRemoved += OnNodeRemoved;

            var nodeViews = this.dynamoWindow.FindVisualChildren<NodeView>().ToList();
            foreach (var nodeView in nodeViews)
            {
                NodeModel model = nodeView.ViewModel.NodeModel;
                this.collector.Add(model.GUID, nodeView);
                
            }
        }

        #region Private Methods
        private void OnNodeRemoved(NodeModel nodeModel)
        {
            this.collector.Remove(nodeModel.GUID);
        }

        private void OnNodeAdded(NodeModel nodeModel)
        {
            lastGuidAdded = nodeModel.GUID;
            dynamoWindow.LayoutUpdated += DynamoWindow_LayoutUpdated;
        }

        private void DynamoWindow_LayoutUpdated(object sender, EventArgs e)
        {
            var nodeViews = this.dynamoWindow.FindVisualChildren<NodeView>().ToList();
            nodeViews.Reverse();
            foreach (var nodeView in nodeViews)
            {
                NodeModel model = nodeView.ViewModel.NodeModel;
                if (model.GUID == this.lastGuidAdded)
                {
                    this.collector.Add(model.GUID, nodeView);
                    break;
                }
            }

            dynamoWindow.LayoutUpdated -= DynamoWindow_LayoutUpdated;
        }
        #endregion

        #region Public Methods

        public List<NodeView> NodeViews
        {
            get => collector.Values.ToList();
        }

        public NodeView Get(Guid guid)
        {
            NodeView nodeView;
            if(this.collector.TryGetValue(guid, out nodeView))
            {
                return nodeView;
            }

            return null;
        }

        #endregion
    }
}
