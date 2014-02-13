using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using DataSources.CodeFlow;

namespace Events.CodeFlow
{
    public class CodeFlowAuthoredEventQueryService : EventQueryServiceBase
    {
        public override IEnumerable<Event> PullEvents(DateTime startDateTime, DateTime stopDateTime,
            Func<Event, bool> predicate)
        {
            var cfdp = new CodeFlowDataProvider();

            var retval = cfdp.PullData(startDateTime, stopDateTime)
                .Select(p =>
                {
                    var e = new Event
                    {
                        Date = p.PublishedUtcDate,
                        Duration = p.ClosedUtcDate - p.PublishedUtcDate,
                        Text = p.Name
                    };

                    e.Participants.Add(new Participant(p.AuthorLogin));

                    return e;
                });
        

    

            return (predicate != null) ? retval.Where(predicate) : retval;
        }
    }
}
