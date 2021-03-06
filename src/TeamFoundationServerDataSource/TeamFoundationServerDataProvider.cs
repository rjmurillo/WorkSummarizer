﻿

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace WorkSummarizer.TeamFoundationServerDataSource
{
    public class TeamFoundationServerDataProvider : ITfsData
    {
        public IEnumerable<Changeset> PullChangesets(Uri tfsConnectionstring, string projectName, DateTime startDate, DateTime endDate)
        {
            try
            {
                VersionSpec versionFrom = new DateVersionSpec(startDate);
                VersionSpec versionTo = new DateVersionSpec(endDate);

                TfsTeamProjectCollection projectCollection =
                    TfsTeamProjectCollectionFactory.GetTeamProjectCollection(tfsConnectionstring);
                VersionControlServer versionControlServer = (VersionControlServer)projectCollection.GetService(typeof(VersionControlServer));
                
                IEnumerable changesetHistory =
                    versionControlServer.QueryHistory("$/" + projectName + "/", VersionSpec.Latest, 0,
                                                      RecursionType.Full, null, versionFrom, versionTo, int.MaxValue,
                                                      false, false);

                return changesetHistory.Cast<Changeset>().ToList();
            }
            catch (Exception ex)
            {
                throw new TeamFoundationException("Unable to get versionControlServer for TFS server " + tfsConnectionstring.AbsoluteUri, ex);
            }
        }

        public IEnumerable<Changeset> PullChangesets(Uri tfsConnectionString, IEnumerable<int> changesetIds)
        {
            TfsTeamProjectCollection projectCollection =
                   TfsTeamProjectCollectionFactory.GetTeamProjectCollection(tfsConnectionString);
            VersionControlServer versionControlServer = (VersionControlServer)projectCollection.GetService(typeof(VersionControlServer));
            
            return changesetIds.Select(versionControlServer.GetChangeset);
        }
        
        public IEnumerable<WorkItem> PullWorkItemsThatChanged(Uri tfsConnectionstring, string projectName, DateTime startDate, DateTime endDate)
        {
            WorkItemStore workItemStore = GetWorkItemStore(tfsConnectionstring);

            const string queryTemplate = @"
                SELECT ID, Title, [Team Project], [Microsoft.VSTS.Common.Priority], System.ChangedDate, [System.AssignedTo], [System.IterationPath], [System.AreaPath], [System.State], [CodeBox.UserVotes]
                FROM Issue 
                WHERE 
                    [System.TeamProject] = @projectName                    
                    and System.ChangedDate > @startDate 
                    and System.ChangedDate < @endDate
                ";
            IDictionary paramsDictionary = new Dictionary<string, object>();
            paramsDictionary["projectName"] = projectName;

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


        public IEnumerable<WorkItem> PullWorkItems(Uri tfsConnectionString, string projectName, IEnumerable<int> workItemIds)
        {
            WorkItemStore workItemStore = GetWorkItemStore(tfsConnectionString);

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
            paramsDictionary["projectName"] = projectName;

            WorkItemCollection tfsWorkItemCollection = workItemStore.Query(query, paramsDictionary);
            return tfsWorkItemCollection.Cast<WorkItem>().ToList();
        }
    }
}
