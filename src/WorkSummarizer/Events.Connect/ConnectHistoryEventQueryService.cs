using System;
using System.Collections.Generic;
using System.Linq;
using DataSources.Connect;

namespace Events.Connect
{
    public class ConnectHistoryEventQueryService : IEventQueryService
    {
        public IEnumerable<Event> PullEvents(DateTime startDateTime, DateTime stopDateTime)
        {
            ConnectDataProvider.PullHistory(startDateTime, stopDateTime);
            return Enumerable.Empty<Event>();
        }
    }
}
