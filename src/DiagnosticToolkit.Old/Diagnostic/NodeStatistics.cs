using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiagnosticToolkit
{

    /// <summary>
    /// Stores a node's performance statistics
    /// </summary>
    public class NodeStatistics
    {
        public string Name { get; set; }

        public Guid GUID { get; set; }

        public string NickName { get; set; }

        public List<PerformanceData> Performance = new List<PerformanceData>();

        public NodeStatistics()
        {
        }

        public NodeStatistics(string Name, Guid guid, string nickname)
        {
            this.Name = Name;
            this.GUID = guid;
            this.NickName = nickname;
        }
    }
}
