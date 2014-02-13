using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataSources.ManicTime;

namespace Events.ManicTime
{
    public class ManicTimeActivityEventQueryService : IEventQueryService
    {
        public IEnumerable<Event> PullEvents(DateTime startDateTime, DateTime stopDateTime)
        {
            ManicTimeDataProvider.PullActivities(startDateTime, stopDateTime);
            return Enumerable.Empty<Event>();
        }
    }
}
