using System.Collections.Generic;

namespace Graph
{
    public static class EnumerableGraphExtensions
    {
        public static Graph<T> ToGraph<T>(this IEnumerable<T> foo) where T : class
        {
            var graph = new Graph<T>();
            foreach (var f in foo)
            {
                graph.Add(f);
            }

            return graph;
        }
    }
}