
using System;
using System.Collections.Generic;
using System.Linq;
using DataSources.Yammer;

namespace Events.Yammer
{
    public class YammerEventsQueryService
    {
        public IEnumerable<Event> PullEvents(DateTime startDateTime, DateTime endDateTime)
        {
            YammerDataProvider.PullSentMessages(startDateTime, endDateTime);
            return Enumerable.Empty<Event>();
        }
    }
}