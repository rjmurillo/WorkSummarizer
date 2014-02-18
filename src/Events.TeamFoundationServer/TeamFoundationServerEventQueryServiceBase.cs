using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.TeamFoundation.Client;

namespace Events.TeamFoundationServer
{
    public abstract class TeamFoundationServerEventQueryServiceBase : EventQueryServiceBase
    {
        protected TeamFoundationServerEventQueryServiceBase(Uri teamFoundationServerUri, string projectName)
        {
            TeamFoundationServer = teamFoundationServerUri;
            Project = projectName;
        }

        public Uri TeamFoundationServer { get; private set; }
        public string Project { get; private set; }

        public override IEnumerable<Event> PullEvents(DateTime startDateTime, DateTime stopDateTime, string alias)
        {
            TfsTeamProjectCollection projectCollection =
                TfsTeamProjectCollectionFactory.GetTeamProjectCollection(TeamFoundationServer);

            var identities = new[]
            {
                projectCollection.TeamFoundationServer.AuthenticatedUserIdentity.AccountName,
                projectCollection.TeamFoundationServer.AuthenticatedUserIdentity.DisplayName,
                projectCollection.TeamFoundationServer.AuthenticatedUserDisplayName,
                projectCollection.TeamFoundationServer.AuthenticatedUserName,
                alias
            };

            return PullEvents(startDateTime, stopDateTime, e => e.Participants.Any(s => identities.Contains(s.Value.Alias, StringComparer.OrdinalIgnoreCase)));
        }
    }
}