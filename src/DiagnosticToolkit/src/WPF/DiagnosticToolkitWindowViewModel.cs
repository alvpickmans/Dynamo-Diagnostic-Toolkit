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

        public static IQueryNodePerformance NodePerformance { get { return statistics; } }

        public void SaveData()
        {
            statistics.Save(statfile);
        }

        public DiagnosticToolkitWindowViewModel(ReadyParams p, DynamoModel model)
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


            NodeViewData = new SeriesCollection
            {
                new ScatterSeries
                {
                    Values = new ChartValues<ScatterPoint>
                    {
                        new ScatterPoint(5, 5, 20),
                        new ScatterPoint(3, 4, 80),
                        new ScatterPoint(7, 2, 20),
                        new ScatterPoint(2, 6, 60),
                        new ScatterPoint(8, 2, 70)
                    },
                    MinPointShapeDiameter = 15,
                    MaxPointShapeDiameter = 45
                },
                new ScatterSeries
                {
                    Values = new ChartValues<ScatterPoint>
                    {
                        new ScatterPoint(7, 5, 1),
                        new ScatterPoint(2, 2, 1),
                        new ScatterPoint(1, 1, 1),
                        new ScatterPoint(6, 3, 1),
                        new ScatterPoint(8, 8, 1)
                    },
                    MinPointShapeDiameter = 15,
                    MaxPointShapeDiameter = 45
                }

            };
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
