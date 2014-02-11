using System.Collections.Generic;

namespace TeamFoundationServerWorkItemStateUpdater.Graph
{
    public class GraphNode<T> : Node<T>
        where T : class
    {
        public GraphNode()
            : base(default(T), new NodeListCollection<T>())
        {
            Costs = new List<int>();
        }

        public GraphNode(T value)
            : base(value)
        {
            Costs = new List<int>();
        }

        public GraphNode(T value, NodeListCollection<T> neighbors)
            : base(value, neighbors)
        {
            Costs = new List<int>();
        }

        public List<int> Costs { get; private set; }
    }
}