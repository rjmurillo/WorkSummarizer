
using System;
using System.Collections.Generic;
using System.Linq;
using Common;
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
                    var e = new Event
                    {
                        Date = p.CreatedAt, 
                        EventType = "Yammer.SentMessages", 
                        Text = p.Body
                    };
                    e.Participants.Add(new Participant(p.Sender));
                    return e;
                });
        }
    }
}