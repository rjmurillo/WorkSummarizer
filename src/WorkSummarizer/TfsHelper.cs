using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using Graph;

namespace WorkSummarizer
{
    public static class TfsHelper
    {
        public static IDictionary<int, Changeset> CreateChangesetIdMap(IEnumerable<Changeset> changesets)
        {
            return changesets.ToDictionary(o => o.ChangesetId, o => o);
        }

        public static IDictionary<int, WorkItem> CreateWorkItemIdMap(IEnumerable<WorkItem> workitems)
        {
            return workitems.ToDictionary(o => o.Id);
        }

        public static Graph<WorkItemNode> BuildWorkItemGraph(IEnumerable<WorkItem> workitems)
        {
            var graph = new Graph<WorkItemNode>();

            var allWorkItemNodes = workitems.Select(o => new WorkItemNode(o)).ToList();

            var rootNodes = allWorkItemNodes.Where(o => !o.HasParent).Select(o => new GraphNode<WorkItemNode>(o)).ToList();

            foreach (var node in rootNodes)
            {
                graph.Add(node);
                BuildChildNodes(node, graph, allWorkItemNodes);
            }

            return graph;
        }
        private static void BuildChildNodes(GraphNode<WorkItemNode> current, Graph<WorkItemNode> graph, IEnumerable<WorkItemNode> workitems)
        {
            var childNodes = workitems.Where(o => current.Value.Id == o.ParentId).Select(o => new GraphNode<WorkItemNode>(o));

            foreach (var childNode in childNodes)
            {
                graph.AddUnidirectedEdge(current, childNode, 1);
                BuildChildNodes(childNode, graph, workitems);
            }
        }
    }
}
