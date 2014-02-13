using System;
using System.Collections.Generic;
using System.Linq;

namespace Events
{
    public abstract class EventQueryServiceBase : IEventQueryService
    {

        public virtual IEnumerable<Event> PullEvents(DateTime startDateTime, DateTime stopDateTime)
        {
            return PullEvents(startDateTime, stopDateTime, e => true);
        }

        public virtual IEnumerable<Event> PullEvents(DateTime startDateTime, DateTime stopDateTime, string alias)
        {
            return PullEvents(startDateTime, stopDateTime, e => e.Participants.Any(s=>s.Value.Alias.Equals(alias, StringComparison.OrdinalIgnoreCase)));
        }

        public abstract IEnumerable<Event> PullEvents(DateTime startDateTime, DateTime stopDateTime,
            Func<Event, bool> predicate);
    }
}