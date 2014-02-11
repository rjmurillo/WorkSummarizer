

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace WorkSummarizer.TeamFoundationServerDataSource
{
    public partial class TfsData : ITfsData
    {
        public IEnumerable<Changeset> PullChangeSets(Uri tfsConnectionstring, DateTime startDate, DateTime endDate)
        {
            try
            {
                VersionSpec versionFrom = new DateVersionSpec(startDate);
                VersionSpec versionTo = new DateVersionSpec(endDate);

                TfsTeamProjectCollection projectCollection =
                    TfsTeamProjectCollectionFactory.GetTeamProjectCollection(tfsConnectionstring);
                VersionControlServer versionControlServer = (VersionControlServer)projectCollection.GetService(typeof(VersionControlServer));
                
                IEnumerable changesetHistory =
                    versionControlServer.QueryHistory("$/", VersionSpec.Latest, 0,
                                                      RecursionType.Full, null, versionFrom, versionTo, int.MaxValue,
                                                      false, false);

                return changesetHistory.Cast<Changeset>().ToList();
            }
            catch (Exception ex)
            {
                throw new TeamFoundationException("Unable to get versionControlServer for TFS server " + tfsConnectionstring.AbsoluteUri, ex);
            }
        }
    }
}
