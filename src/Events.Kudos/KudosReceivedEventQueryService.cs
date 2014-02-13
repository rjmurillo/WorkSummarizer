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
            var kdp = new KudosDataProvider();

            return kdp.PullData(startDateTime, stopDateTime)
                .Select(p => 
                {
                    var e = new Event
                    {
                        Date = p.CreatedUtcTime,
                        EventType = "Kudos.Received",
                        Subject = new Subject { Text = p.SenderAlias },
                        Text = p.Message
                    };

                    e.Participants.Add(new Participant(p.SenderAlias));

                    return e;
                });
        }
    }
}
