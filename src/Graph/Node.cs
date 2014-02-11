namespace TeamFoundationServerWorkItemStateUpdater.Graph
{
    public class Node<T>
        where T : class
    {
        public T Value { get; set; }

        public NodeListCollection<T> Neighbors { get; private set; }

        public Node()
        {
        }

        public Node(T value)
            : this(value, null)
        {
        }

        public Node(T value, NodeListCollection<T> neighbors)
        {
            Value = value;
            Neighbors = neighbors ?? new NodeListCollection<T>();
        }
    }
}