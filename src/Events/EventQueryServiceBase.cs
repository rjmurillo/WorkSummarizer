using System;
using System.Collections.Generic;

namespace Events
{
    public abstract class EventQueryServiceBase : IEventQueryService
    {

        public virtual IEnumerable<Event> PullEvents(DateTime startDateTime, DateTime stopDateTime)
        {
            return PullEvents(startDateTime, stopDateTime, null);
        }

        public abstract IEnumerable<Event> PullEvents(DateTime startDateTime, DateTime stopDateTime,
            Func<Event, bool> predicate);
    }
}