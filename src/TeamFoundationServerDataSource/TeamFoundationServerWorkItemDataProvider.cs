using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using WorkSummarizer.TeamFoundationServerDataSource;

namespace DataSources.TeamFoundationServer
{
    public class TeamFoundationServerWorkItemDataProvider : IDataPull<WorkItem>
    {

        public TeamFoundationServerWorkItemDataProvider(Uri tfsConnectionString, string projectName)
        {
            TeamFoundationServer = tfsConnectionString;
            Project = projectName;
        }

        public Uri TeamFoundationServer { get; private set; }
        public string Project { get; private set; }

        public IEnumerable<WorkItem> PullData(DateTime startDate, DateTime endDate)
        {
            WorkItemStore workItemStore = GetWorkItemStore(TeamFoundationServer);

            const string queryTemplate = @"
                SELECT ID, Title, [Team Project], [Microsoft.VSTS.Common.Priority], System.ChangedDate, [System.AssignedTo], [System.IterationPath], [System.AreaPath], [System.State], [CodeBox.UserVotes]
                FROM Issue 
                WHERE 
                    [System.TeamProject] = @projectName                    
                    and System.ChangedDate > @startDate 
                    and System.ChangedDate < @endDate
                ";
            IDictionary paramsDictionary = new Dictionary<string, object>();
            paramsDictionary["projectName"] = Project;

            //TFS will throw an error if you use the time in query and it defaults to midnight on the day sent in so we 
            //have to use the previous day's date.
            paramsDictionary["startDate"] = startDate.Date.AddDays(-1);
            paramsDictionary["endDate"] = endDate.Date.AddDays(1);

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


        public IEnumerable<WorkItem> PullWorkItems(IEnumerable<int> workItemIds)
        {
            WorkItemStore workItemStore = GetWorkItemStore(TeamFoundationServer);

            const string queryTemplate = @"
                SELECT ID, Title, [Team Project], [Microsoft.VSTS.Common.Priority], System.ChangedDate, [System.AssignedTo], [System.IterationPath], [System.AreaPath], [System.State], [CodeBox.UserVotes]
                FROM Issue 
                WHERE 
                    [System.TeamProject] = @projectName                    
                    and ([System.ID]={0})
                ";

            var thing = string.Join(" OR [System.ID]=", workItemIds);

            var query = (string.Format(CultureInfo.InvariantCulture, queryTemplate, thing));

            IDictionary paramsDictionary = new Dictionary<string, object>();
            paramsDictionary["projectName"] = Project;

            WorkItemCollection tfsWorkItemCollection = workItemStore.Query(query, paramsDictionary);
            return tfsWorkItemCollection.Cast<WorkItem>().ToList();
        }
    }
}
