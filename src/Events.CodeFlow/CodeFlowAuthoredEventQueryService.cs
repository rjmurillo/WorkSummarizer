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
            return CodeFlowDataProvider.PullReviewsAuthored(startDateTime, stopDateTime)
                .Select(p => new Event
                {
                    Date = p.PublishedUtcDate,
                    Duration = (p.ClosedUtcDate - p.PublishedUtcDate).TotalMinutes,
                    Subject = new Subject { Text = p.AuthorLogin },
                    Text = p.Name
                });
        }
    }
}
