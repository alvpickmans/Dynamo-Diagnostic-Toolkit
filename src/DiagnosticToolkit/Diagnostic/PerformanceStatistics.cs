using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Dynamo.Graph.Nodes;
using Dynamo.Graph.Nodes.CustomNodes;
using Dynamo.Graph.Nodes.ZeroTouch;
using Newtonsoft.Json;

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

    public interface IQueryNodePerformance
    {
        IEnumerable<PerformanceData> GetNodePerformance(NodeModel node);
    }

    /// <summary>
    /// Stores performance statistics of all nodes.
    /// </summary>
    public class PerformanceStatistics : IQueryNodePerformance
    {
        private Dictionary<Guid, NodeStatistics> statistics { get; set; }

        public PerformanceStatistics()
        {
            statistics = new Dictionary<Guid, NodeStatistics>();
        }

        public void AddPerformanceData(NodeModel node, PerformanceData data)
        {
            var name = GetUniqueNodeName(node);
            NodeStatistics stats;
            if (!statistics.TryGetValue(node.GUID, out stats))
            {
                stats = new NodeStatistics(name, node.GUID, node.Name);
            }
            
            stats.Performance.Add(data);
            statistics[node.GUID] = stats;
        }

        public IEnumerable<PerformanceData> GetNodePerformance(NodeModel node)
        {
            var name = GetUniqueNodeName(node);
            NodeStatistics stats;
            if (statistics.TryGetValue(node.GUID, out stats))
                return stats.Performance;

            return Enumerable.Empty<PerformanceData>();
        }

        public string GetUniqueNodeName(NodeModel node)
        {
            if (node is DSFunction)
                return node.CreationName;

            var codeblock = node as CodeBlockNodeModel;
            if (codeblock != null)
                return "CodeBlock_" + codeblock.Code.GetHashCode();

            var customNode = node as Function;
            if (customNode != null)
                return customNode.Definition.DisplayName;

            var name = node.Name;
            if (!string.IsNullOrEmpty(name))
                return name;

            return node.GetType().FullName;
        }

        public List<NodeStatistics> Data 
        { 
            get
            {
                return statistics.Values.ToList();
            }
            set
            {
                if (value != null)
                {
                    statistics = value.ToDictionary(x => x.GUID);
                }
            }
        }

        public static PerformanceStatistics Load(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                return null;

            try
            {
                string file = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<PerformanceStatistics>(file);
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Failed to load PerformanceStatistics from file {0}", filePath));
                Console.Write(ex.Message);
            }

            return null;
        }

        public void Save(string filePath)
        {
            using (StreamWriter file = File.CreateText(filePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, this);
            }
        }
    }
}
