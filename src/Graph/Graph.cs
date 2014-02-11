namespace TeamFoundationServerWorkItemStateUpdater.Graph
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public class Graph<T> : IEnumerable<Node<T>>
        where T : class
    {
        public NodeListCollection<T> Nodes { get; private set; }

        public int Count
        {
            get { return Nodes.Count; }
        }

        public void Add(GraphNode<T> node)
        {
            if (!Nodes.Contains(node))
            {
                Nodes.Add(node);
            }
        }

        public GraphNode<T> Add(T element)
        {
            var n = (GraphNode<T>)Nodes.FindByValue(element);

            if (n == null)
            {
                n = new GraphNode<T>(element);
                Nodes.Add(n);
            }

            return n;
        }

        public void AddRange(IEnumerable<GraphNode<T>> data)
        {
            foreach (GraphNode<T> n in data)
            {
                Add(n);
            }
        }

        public void AddDirectedEdge(GraphNode<T> from, GraphNode<T> to, int cost)
        {
            if (!from.Neighbors.Contains(to))
            {
                from.Neighbors.Add(to);
                from.Costs.Add(cost);
            }
        }

        public void AddUnidirectedEdge(GraphNode<T> from, GraphNode<T> to, int cost)
        {
            AddDirectedEdge(from, to, cost);
            AddDirectedEdge(to, from, cost);
        }

        public void Union2(Graph<T> graph)
        {
            foreach (GraphNode<T> n in graph.Nodes.OfType<GraphNode<T>>())
            {
                Add(n);
                for (int i = 0; i < n.Neighbors.Count; i++)
                {
                    var nb = n.Neighbors[i] as GraphNode<T>;
                    AddDirectedEdge(n, nb, n.Costs[i]);
                }
            }
        }

        public bool Contains(T value)
        {
            return Nodes.FindByValue(value) != null;
        }

        //public bool Remove(T value)
        //{
        //    // Locate the node in the node set
        //    var node = (GraphNode<T>)Nodes.FindByValue(value);
        //    if (node == null)
        //        return false;

        //    Nodes.Remove(node);

        //    // Remove all the edges
        //    foreach (var gn in Nodes)
        //    {
        //        int i = gn.Neighbors.IndexOf(node);
        //        if (i != -1)
        //        {
        //            gn.Neighbors.RemoveAt(i);

        //            gn.Costs.RemoveAt(i);
        //        }
        //    }

        //    return true;
        //}

        public Graph()
        {
            Nodes = new NodeListCollection<T>();
        }

        public Graph(NodeListCollection<T> nodes)
        {
            Nodes = nodes ?? new NodeListCollection<T>();
        }

        public Graph(IEnumerable<GraphNode<T>> data)
            : this()
        {
            AddRange(data);
        }

        public IEnumerator<Node<T>> GetEnumerator()
        {
            return Nodes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Nodes.GetEnumerator();
        }
    }
}