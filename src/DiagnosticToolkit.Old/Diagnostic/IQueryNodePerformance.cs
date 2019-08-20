using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dynamo.Graph.Nodes;

namespace DiagnosticToolkit
{
    public interface IQueryNodePerformance
    {
        IEnumerable<PerformanceData> GetNodePerformance(NodeModel node);
    }
}
