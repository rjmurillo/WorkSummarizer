

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace WorkSummarizer.TeamFoundationServerDataSource
{
    public partial class TfsData
    {
        public IEnumerable<WorkItem> PullWorkItemsThatChanged(Uri tfsConnectionstring, DateTime startDate, DateTime endDate)
        {
            WorkItemStore workItemStore = GetWorkItemStore(tfsConnectionstring);

            const string queryTemplate = @"
                SELECT ID, Title, [Team Project], [Microsoft.VSTS.Common.Priority], System.ChangedDate, [System.AssignedTo], [System.IterationPath], [System.AreaPath], [System.State], [CodeBox.UserVotes]
                FROM Issue 
                WHERE System.ChangedDate > @startDate and System.ChangedDate < @endDate
                ";
            IDictionary paramsDictionary = new Dictionary<string, object>();
            //TFS will throw an error if you use the time in query and it defaults to midnight on the day sent in so we 
            //have to use the previous day's date.
            paramsDictionary["startDate"] = startDate.Date.AddDays(-1);
            paramsDictionary["endDate"] = startDate.Date.AddDays(1);

            WorkItemCollection tfsWorkItemCollection = workItemStore.Query(queryTemplate, paramsDictionary);

            return tfsWorkItemCollection.Cast<WorkItem>().ToList();
        }

        private static WorkItemStore GetWorkItemStore(Uri tfsConnectionString)
        {
            try
            {
                TfsTeamProjectCollection projectCollection = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(tfsConnectionString);
                return projectCollection.GetService(typeof(WorkItemStore)) as WorkItemStore;
            }
            catch (WebException ex)
            {
                throw new TeamFoundationException("Unable to get WorkItemStore for TFS server " + tfsConnectionString.AbsolutePath, ex);
            }
            catch (ConnectionException ex)
            {
                throw new TeamFoundationException("Unable to get WorkItemStore for TFS server " + tfsConnectionString.AbsolutePath, ex);
            }
            catch (TeamFoundationServiceUnavailableException ex)
            {
                throw new TeamFoundationException("Unable to get WorkItemStore for TFS server " + tfsConnectionString.AbsolutePath, ex);
            }
        }
    }
}
