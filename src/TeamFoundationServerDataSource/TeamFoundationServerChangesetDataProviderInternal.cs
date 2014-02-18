using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using WorkSummarizer.TeamFoundationServerDataSource;

namespace DataSources.TeamFoundationServer
{
    internal class TeamFoundationServerChangesetDataProviderInternal : IDataPull<Changeset>
    {
        public TeamFoundationServerChangesetDataProviderInternal(Uri tfsConnectionString, string projectName)
        {
            TeamFoundationServer = tfsConnectionString;
            Project = projectName;
        }

        public Uri TeamFoundationServer { get; private set; }
        public string Project { get; private set; }

        public IEnumerable<Changeset> PullData(DateTime startDate, DateTime endDate)
        {
            try
            {
                VersionSpec versionFrom = new DateVersionSpec(startDate);
                VersionSpec versionTo = new DateVersionSpec(endDate);

                TfsTeamProjectCollection projectCollection =
                    TfsTeamProjectCollectionFactory.GetTeamProjectCollection(TeamFoundationServer);
                VersionControlServer versionControlServer =
                    (VersionControlServer)projectCollection.GetService(typeof(VersionControlServer));

                string scope = null;
                if (string.IsNullOrWhiteSpace(Project))
                {
                    scope = "$/*";
                }
                else
                {
                    scope = "$/" + Project + "/";
                }

                IEnumerable changesetHistory =
                    versionControlServer.QueryHistory(
                        scope,
                        VersionSpec.Latest,
                        0,
                        RecursionType.Full,
                        null,
                        versionFrom,
                        versionTo,
                        int.MaxValue,
                        false,
                        false);

                return changesetHistory.Cast<Changeset>().ToList();
            }
            catch (Exception ex)
            {
                throw new TeamFoundationException(
                    "Unable to get versionControlServer for TFS server " + TeamFoundationServer.AbsoluteUri, ex);
            }
        }

        public IEnumerable<Changeset> PullChangesets(Uri tfsConnectionString, IEnumerable<int> changesetIds)
        {
            TfsTeamProjectCollection projectCollection =
                TfsTeamProjectCollectionFactory.GetTeamProjectCollection(tfsConnectionString);
            VersionControlServer versionControlServer =
                (VersionControlServer)projectCollection.GetService(typeof(VersionControlServer));

            return changesetIds.Select(versionControlServer.GetChangeset);
        }
    }
}