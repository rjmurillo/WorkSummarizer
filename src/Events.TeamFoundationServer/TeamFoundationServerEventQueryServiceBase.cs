using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.TeamFoundation;
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

            try
            {
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
            catch (TeamFoundationServerUnauthorizedException)
            {
                Trace.WriteLine("TFS server failed authentication");
                return Enumerable.Empty<Event>();
            }
        }
    }
}