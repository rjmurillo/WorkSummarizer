
using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using DataSources.Who;
using DataSources.Yammer;

namespace Events.Yammer
{
    public class YammerEventsQueryService : EventQueryServiceBase
    {
        public override IEnumerable<Event> PullEvents(DateTime startDateTime, DateTime endDateTime, Func<Event, bool> predicate )
        {
            var dp = new YammerDataProvider();

            var retval = dp.PullData(startDateTime, endDateTime)
                .Select(p =>
                {
                    var e = new Event
                    {
                        Date = p.CreatedAt, 
                        EventType = "Yammer.SentMessages", 
                        Text = p.Body
                    };
                    e.Participants.Add(IdentityUtility.Create(p.Sender));
                    return e;
                });

            return (predicate != null) ? retval.Where(predicate) : retval;
        }
    }
}