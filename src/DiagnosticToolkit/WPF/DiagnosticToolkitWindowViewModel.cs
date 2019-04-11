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

namespace DiagnosticToolkit
{
    public class DiagnosticToolkitWindowViewModel : NotificationObject, IDisposable
    {
        private ReadyParams readyParams;
        private DynamoModel dynamoModel;
        private DiagnosticsSession session;
        private string statfile;
        private static PerformanceStatistics statistics = new PerformanceStatistics();
        private static WorkspaceModel ws;

        private NodeViewCollector nodeViewCollector;
        private Window _diagnosticWindow;

        #region UI Properties
        private SeriesCollection nodeViewData { get; set; }
        public SeriesCollection NodeViewData
        {
            get => nodeViewData;
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

        public DiagnosticToolkitWindowViewModel(ViewLoadedParams p, DynamoModel model)
        {
            readyParams = p;
            dynamoModel = model;
            HomeWorkspaceModel homeWorkspaceModel = p.CurrentWorkspaceModel as HomeWorkspaceModel;
            
            //Creates statistic XML files, if file exsists load that.
            statfile = Path.Combine(model.PathManager.UserDataDirectory, "Statistics.json");
            if (File.Exists(statfile))
                statistics = PerformanceStatistics.Load(statfile);
            ws = readyParams.WorkspaceModels.OfType<HomeWorkspaceModel>().First();
            SetupDiagnosticsSession(ws);

            homeWorkspaceModel.EvaluationCompleted += Hwm_EvaluationCompleted;

            session.SessionExecuted += Session_SessionExecuted;

            this.nodeViewCollector = new NodeViewCollector(p);

            NodeViewData = new SeriesCollection();
        }

        public void AssignWindow(Window window)
        {
            _diagnosticWindow = window;
        }

        private void UpdateNodeViewData (DiagnosticsSession session)
        {
            _diagnosticWindow.Dispatcher.Invoke(() => {

                int minDiameter = session.EvaluatedNodes.Min(nd => nd.ExecutionTime);
                int maxDiameter = session.EvaluatedNodes.Max(nd => nd.ExecutionTime);
                int minimum = 10;
                int maximum = 50;

                List<ScatterPoint> points = nodeViewCollector.NodeViews.Select(nv =>
                {
                    Point location = nv.GetLocation();
                    NodeData nodeData = session.EvaluatedNodes.FirstOrDefault(nd => nd.Node.GUID == nv.ViewModel.NodeModel.GUID);
                    var time = nodeData == null ? 0 : nodeData.ExecutionTime;
                    var diameter = time.Map(minDiameter, maxDiameter, minimum, maximum);

                    nv.AddTime(time);

                    ScatterPoint p = new ScatterPoint(location.X, -location.Y, diameter);

                    return p;
                }).ToList();

                this.NodeViewData = new SeriesCollection
                {
                    new ScatterSeries
                    {
                        Values = new ChartValues<ScatterPoint>(points),
                        MinPointShapeDiameter = minimum,
                        MaxPointShapeDiameter = maximum
                    }

                };
            });
        }

        private void Session_SessionExecuted(object sender, EventArgs e)
        {
            DiagnosticsSession session = sender as DiagnosticsSession;

            this.ExecutionTime = session.ExecutionTime;
            this.ExecutedNodes = session.EvaluatedNodes.Count;

            this.UpdateNodeViewData(session);

        }

        private void Hwm_EvaluationCompleted(object sender, EvaluationCompletedEventArgs e)
        {
            DiagnosticsSession test = session;
            PerformanceStatistics statTest = statistics;
        }

        private void SetupDiagnosticsSession(IWorkspaceModel model)
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

        }
    }
}
