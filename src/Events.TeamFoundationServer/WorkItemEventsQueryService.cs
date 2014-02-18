using System;
using System.Collections.Generic;
using DataSources.TeamFoundationServer;

namespace Events.TeamFoundationServer
{
    public class WorkItemEventsQueryService : IEventQueryService
    {
        public IEnumerable<Event> PullEvents(DateTime startDateTime, DateTime stopDateTime)
        {
            List<Event> retval = new List<Event>();

            var servers = TeamFoundationServerRegistrationUtility.LoadRegisteredTeamFoundationServers();

            foreach (var server in servers)
            {
                var q = new WorkItemEventsQueryServiceInternal(server, null);
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
                var q = new WorkItemEventsQueryServiceInternal(server, null);
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
                var q = new WorkItemEventsQueryServiceInternal(server, null);
                retval.AddRange(q.PullEvents(startDateTime, stopDateTime, predicate));
            }

            return retval;
        }
    }
}