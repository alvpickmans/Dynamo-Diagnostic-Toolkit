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

        public static IQueryNodePerformance NodePerformance { get { return statistics; } }

        public DiagnosticToolkitWindowViewModel(ReadyParams p, DynamoModel model)
        {
            readyParams = p;
            dynamoModel = model;
            HomeWorkspaceModel homeWorkspaceModel = p.CurrentWorkspaceModel as HomeWorkspaceModel;

            //Creates statistic XML files, if file exsists load that.
            statfile = Path.Combine(model.PathManager.UserDataDirectory, "Statistics.xml");
            if (File.Exists(statfile))
                statistics = PerformanceStatistics.Load(statfile);
            ws = readyParams.WorkspaceModels.OfType<HomeWorkspaceModel>().First();
            SetupDiagnosticsSession(ws);

            homeWorkspaceModel.EvaluationCompleted += Hwm_EvaluationCompleted;

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
