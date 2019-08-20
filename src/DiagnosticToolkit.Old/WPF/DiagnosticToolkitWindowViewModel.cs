using System;
using System.IO;
using System.Linq;
using Dynamo.Core;
using Dynamo.Engine.CodeGeneration;
using Dynamo.Events;
using Dynamo.Extensions;
using Dynamo.Graph.Nodes;
using Dynamo.Graph.Workspaces;
using Dynamo.Models;
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Defaults;
using DiagnosticToolkit.Utilities;
using Dynamo.Wpf.Extensions;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Threading;
using Dynamo.ViewModels;

namespace DiagnosticToolkit
{
    public class DiagnosticToolkitWindowViewModel : NotificationObject, IDisposable
    {
        private readonly ViewLoadedParams viewParameters;
        private readonly Window dynamoWindow;
        private readonly DynamoViewModel dynamoViewModel;
        private readonly DynamoModel dynamoModel;
        private DiagnosticsSession session;
        private string statfile;
        private static PerformanceStatistics statistics = new PerformanceStatistics();

        private NodeViewCollector nodeViewCollector;

        private double MinDiameter = 5;
        private double MaxDiameter = 50;

        #region UI Properties
        private SeriesCollection nodeViewData { get; set; }
        public SeriesCollection NodeViewData
        {
            get {
                return new SeriesCollection
                {
                    new ScatterSeries
                    {
                        Values = new ChartValues<ScatterPoint>(this.session.Nodes),
                        MinPointShapeDiameter = this.MinDiameter,
                        MaxPointShapeDiameter = this.MaxDiameter
                    }

                };
            }
            set
            {
                nodeViewData = value;
                RaisePropertyChanged("NodeViewData");
            }
        } 

        private int _executionTime { get; set; }
        public int ExecutionTime
        {
            get => _executionTime;
            set
            {
                _executionTime = value;
                RaisePropertyChanged("ExecutionTime");
            }
        }

        private int _executedNodes { get; set; }
        public int ExecutedNodes
        {
            get => _executedNodes;
            set
            {
                _executedNodes = value;
                RaisePropertyChanged("ExecutedNodes");
            }
        }
        #endregion

        public static IQueryNodePerformance NodePerformance { get { return statistics; } }

        public void SaveData()
        {
            statistics.Save(statfile);
        }

        public DiagnosticToolkitWindowViewModel(ViewLoadedParams parameters)
        {
            this.viewParameters = parameters;
            this.nodeViewCollector = new NodeViewCollector(parameters);
            this.dynamoWindow = this.viewParameters.DynamoWindow;
            this.dynamoViewModel = this.dynamoWindow.DataContext as DynamoViewModel;
            this.dynamoModel = this.dynamoViewModel.Model;
            
            
            //Creates statistic json file, if file exists load that.
            statfile = Path.Combine(this.dynamoModel.PathManager.UserDataDirectory, "Statistics.json");
            if (File.Exists(statfile))
            {
                statistics = PerformanceStatistics.Load(statfile);
            }

            SetupSession(viewParameters.CurrentWorkspaceModel);
            NodeViewData = new SeriesCollection();

            RegisterEvents();
        }

        private void RegisterEvents()
        {
            this.dynamoModel.WorkspaceAdded += OnWorkspaceChanged;
            this.session.SessionExecuted += OnSessionExecuted;
            this.session.NodeDataAdded += OnNodeDataUpdated;
            this.session.NodeDataRemoved += OnNodeDataUpdated;
        }

        private void OnNodeDataUpdated(object sender, EventArgs e)
        {
            RaisePropertyChanged("NodeViewData");
        }

        private void UnregisterEvents()
        {
            this.dynamoModel.WorkspaceAdded -= OnWorkspaceChanged;
            this.session.SessionExecuted -= OnSessionExecuted;
        }

        private void OnWorkspaceChanged(WorkspaceModel model)
        {
            this.SetupSession(model);
        }

        private void UpdateNodeViewData (DiagnosticsSession session)
        {
            this.dynamoWindow.Dispatcher.Invoke(() => {

                double minDiameter = session.Nodes.Count < 2 ? this.MinDiameter : session.Nodes.Min(nd => nd.ExecutionTime);
                double maxDiameter = session.Nodes.Count < 2 ? this.MaxDiameter : session.Nodes.Max(nd => nd.ExecutionTime);


                nodeViewCollector.NodeViews.ForEach(nv =>
                {
                    NodeData nodeData = session.GetNodeDataFromGuid(nv.ViewModel.NodeModel.GUID);
                    nodeData.Weight = nodeData.Weight.Map(minDiameter, maxDiameter, this.MinDiameter, this.MaxDiameter);

                    nv.AddTime(nodeData.ExecutionTime);

                });


                RaisePropertyChanged("NodeViewData");
            });
        }

        private void OnSessionExecuted(object sender, EventArgs e)
        {
            DiagnosticsSession session = sender as DiagnosticsSession;

            this.ExecutionTime = session.ExecutionTime;
            this.ExecutedNodes = session.EvaluatedNodes.Count;

            this.UpdateNodeViewData(session);

        }

        private void SetupSession(IWorkspaceModel model)
        {
            if (session != null)
            {
                if (session.WorkSpace == model) return;

                session.Dispose();
                session = null;
            }
            if (model != null)
            {
                session = new DiagnosticsSession(model, statistics);
            }
        }

        public void Dispose()
        {
            UnregisterEvents();
        }


        //private void CartesianChart_DataClick(object sender, ChartPoint chartPoint)
        //{
        //    var node = this.nodeViewCollector.Collector.Where(kvp )
        //    Selection.ZoomToObject(this.dynamoViewModel, )
        //}
    }
}
