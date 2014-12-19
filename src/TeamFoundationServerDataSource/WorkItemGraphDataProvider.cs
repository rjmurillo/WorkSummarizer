using System;
using System.Collections.Generic;
using Graph;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using WorkSummarizer;

namespace DataSources.TeamFoundationServer
{
    public class WorkItemGraphDataProvider : IDataPull<Node<WorkItemNode>>
    {
        private readonly IDataPull<WorkItem> m_data;

        public WorkItemGraphDataProvider(IDataPull<WorkItem> data)
        {
            m_data = data;
        }

        public IEnumerable<Node<WorkItemNode>> PullData(DateTime startDateTime, DateTime endDateTime)
        {
            var wi = m_data.PullData(startDateTime, endDateTime);
            return TfsHelper.BuildWorkItemGraph(wi);
        }
    }
}