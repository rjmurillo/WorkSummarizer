using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using DataSources.CodeFlow;

namespace Events.CodeFlow
{
    public class CodeFlowAuthoredEventQueryService : IEventQueryService
    {
        public IEnumerable<Event> PullEvents(DateTime startDateTime, DateTime stopDateTime)
        {
            var cfdp = new CodeFlowDataProvider();

            return cfdp.PullData(startDateTime, stopDateTime)
                .Select(p => new Event
                {
                    Date = p.PublishedUtcDate,
                    Duration = p.ClosedUtcDate - p.PublishedUtcDate,
                    Subject = new Subject { Text = p.AuthorLogin },
                    Text = p.Name
                });
        }
    }
}
