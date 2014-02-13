using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using DataSources.Kudos;

namespace Events.Kudos
{
    public class KudosReceivedEventQueryService : IEventQueryService
    {
        public IEnumerable<Event> PullEvents(DateTime startDateTime, DateTime stopDateTime)
        {
            return KudosDataProvider.PullReceivedKudos(startDateTime, stopDateTime)
                .Select(p => 
                {
                    return new Event
                    {
                        Date = p.CreatedUtcTime,
                        EventType = "Kudos.Received",
                        Subject = new Subject { Text = p.SenderAlias },
                        Text = p.Message
                    }; 
                });
        }
    }
}
