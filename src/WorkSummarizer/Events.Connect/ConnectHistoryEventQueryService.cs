using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using DataSources.Connect;

namespace Events.Connect
{
    public class ConnectHistoryEventQueryService : IEventQueryService
    {
        public IEnumerable<Event> PullEvents(DateTime startDateTime, DateTime stopDateTime)
        {
            return ConnectDataProvider.PullHistory(startDateTime, stopDateTime)
                .Select(p => { return new Event { Date = p.SubmittedUtcDate, EventType = "Connect.Submission", Subject = new Subject { Text = p.Title }, Text = p.Title }; });
        }
    }
}
