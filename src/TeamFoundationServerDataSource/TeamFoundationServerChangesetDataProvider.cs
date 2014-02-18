using System;
using System.Collections.Generic;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace DataSources.TeamFoundationServer
{
    public class TeamFoundationServerChangesetDataProvider : IDataPull<Changeset>
    {
        public IEnumerable<Changeset> PullData(DateTime startDateTime, DateTime endDateTime)
        {
            List<Changeset> retval = new List<Changeset>();

            foreach (var connection in TeamFoundationServerRegistrationUtility.LoadRegisteredTeamFoundationServers())
            {
                var t = new TeamFoundationServerChangesetDataProviderInternal(connection, null);
                retval.AddRange(t.PullData(startDateTime, endDateTime));
            }

            return retval;
        }
    }
}