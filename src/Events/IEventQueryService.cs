using System;
using System.Collections.Generic;

namespace Events
{
    public interface IEventQueryService
    {
        IEnumerable<Event> PullEvents(DateTime startDateTime, DateTime stopDateTime);

        IEnumerable<Event> PullEvents(DateTime startDateTime, DateTime stopDateTime, string alias);
        IEnumerable<Event> PullEvents(DateTime startDateTime, DateTime stopDateTime, Func<Event, bool> predicate);
    }
}