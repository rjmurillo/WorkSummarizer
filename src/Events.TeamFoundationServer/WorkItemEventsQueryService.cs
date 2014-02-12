using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Events;
using Microsoft.TeamFoundation;

namespace TfsCodeSwarm
{
    public class WorkItemEventsQueryService : IEventQueryService
    {
        public IEnumerable<Event> PullEvents(DateTime startDateTime, DateTime endDateTime)
        {
            throw new NotImplementedException();
        }
    }
}
