namespace Graph
{
    using System.Collections.ObjectModel;
    using System.Linq;

    public class NodeListCollection<T> : Collection<Node<T>>
        where T : class
    {
        public NodeListCollection()
        {
        }

        public NodeListCollection(int size)
        {
            for (int i = 0; i < size; i++)
            {
                Items.Add(default(Node<T>));
            }
        }

        public Node<T> FindByValue(T value)
        {
            return Items.FirstOrDefault(n => n.Value.Equals(value) || n.Value == value);
        }

        public void Add(T element)
        {
            //var n = FindByValue(element.Value);
            //if (n == null)
            //{
            //    Items.Add(element);
            //}
        }
    }
}