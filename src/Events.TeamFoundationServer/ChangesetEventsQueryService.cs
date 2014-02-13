using System;
using System.Collections.Generic;
using System.Linq;

namespace Events.TeamFoundationServer
{
    public class ChangesetEventsQueryService : IEventQueryService
    {
        public IEnumerable<Event> PullEvents(DateTime startDateTime, DateTime stopDateTime)
        {
            return Enumerable.Empty<Event>();
        }
    }
}
