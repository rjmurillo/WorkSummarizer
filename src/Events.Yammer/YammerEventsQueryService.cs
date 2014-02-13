
using System;
using System.Collections.Generic;
using System.Linq;
using DataSources.Yammer;

namespace Events.Yammer
{
    public class YammerEventsQueryService : IEventQueryService
    {
        public IEnumerable<Event> PullEvents(DateTime startDateTime, DateTime endDateTime)
        {
            return YammerDataProvider.PullSentMessages(startDateTime, endDateTime)
                .Select(p =>
                {
                    return new Event {Date = p.CreatedAt, EventType = "Yammer.SentMessages", Text = p.Body};
                });
        }
    }
}