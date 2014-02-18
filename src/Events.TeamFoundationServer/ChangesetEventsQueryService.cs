using Common;
using System;
using System.Collections.Generic;
using DataSources.TeamFoundationServer;
using WorkSummarizer.TeamFoundationServerDataSource;

namespace Events.TeamFoundationServer
{
    public class ChangesetEventsQueryService : IEventQueryService
    {
        public IEnumerable<Event> PullEvents(DateTime startDateTime, DateTime stopDateTime)
        {
            List<Event> retval = new List<Event>();

            var servers = TeamFoundationServerRegistrationUtility.LoadRegisteredTeamFoundationServers();

            foreach (var server in servers)
            {
                var q = new ChangesetEventsQueryServiceInternal(server, null);
                retval.AddRange(q.PullEvents(startDateTime, stopDateTime));
            }

            return retval;
        }

        public IEnumerable<Event> PullEvents(DateTime startDateTime, DateTime stopDateTime, string alias)
        {
            List<Event> retval = new List<Event>();

            var servers = TeamFoundationServerRegistrationUtility.LoadRegisteredTeamFoundationServers();

            foreach (var server in servers)
            {
                var q = new ChangesetEventsQueryServiceInternal(server, null);
                retval.AddRange(q.PullEvents(startDateTime, stopDateTime, alias));
            }

            return retval;
        }

        public IEnumerable<Event> PullEvents(DateTime startDateTime, DateTime stopDateTime, Func<Event, bool> predicate)
        {
            List<Event> retval = new List<Event>();

            var servers = TeamFoundationServerRegistrationUtility.LoadRegisteredTeamFoundationServers();

            foreach (var server in servers)
            {
                var q = new ChangesetEventsQueryServiceInternal(server, null);
                retval.AddRange(q.PullEvents(startDateTime, stopDateTime, predicate));
            }

            return retval;
        }
    }
}
