using System;
using System.Collections.Generic;

namespace Events
{
    public interface IEventQueryService
    {
        IEnumerable<Event> PullEvents(DateTime startDateTime, DateTime stopDateTime);
    }
}