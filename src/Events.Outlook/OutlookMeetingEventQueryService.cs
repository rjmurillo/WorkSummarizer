using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Outlook
{
    public class OutlookMeetingEventQueryService : IEventQueryService
    {
        public IEnumerable<Event> PullEvents(DateTime startDateTime, DateTime stopDateTime)
        {
            throw new NotImplementedException();
        }
    }
}
