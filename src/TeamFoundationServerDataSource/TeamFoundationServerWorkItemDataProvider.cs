using System;
using System.Collections.Generic;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace DataSources.TeamFoundationServer
{
    public class TeamFoundationServerWorkItemDataProvider : IDataPull<WorkItem>{
public IEnumerable<WorkItem> PullData(DateTime startDateTime, DateTime endDateTime)
{
    List<WorkItem> retval = new List<WorkItem>();

    foreach (var connection in TeamFoundationServerRegistrationUtility.LoadRegisteredTeamFoundationServers())
            {
                var t = new TeamFoundationServerWorkItemDataProviderInternal(connection, null);
                retval.AddRange(t.PullData(startDateTime, endDateTime));
            }

    return retval;
}
}
}
