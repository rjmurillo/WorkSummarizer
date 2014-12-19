using System.Collections.Generic;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace DataSources.TeamFoundationServer
{
    public interface IWorkItemDataPull : IDataPull<WorkItem>
    {
        IEnumerable<WorkItem> PullWorkItems(IEnumerable<int> workItemIds);
    }
}