using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using DataSources.Connect;

namespace Events.Connect
{
    public class ConnectHistoryEventQueryService : EventQueryServiceBase
    {
        public override IEnumerable<Event> PullEvents(DateTime startDateTime, DateTime stopDateTime, Func<Event, bool> predicate)
        {
            var cdp = new ConnectDataProvider();

            var retval = cdp.PullData(startDateTime, stopDateTime)
                .Select(p =>
                {
                    return new Event
                    {
                        Date = p.SubmittedUtcDate,
                        EventType = "Connect.Submission",
                        Text = p.Title
                    };
                });

            return (predicate != null) ? retval.Where(predicate) : retval;
        }
    }
}
