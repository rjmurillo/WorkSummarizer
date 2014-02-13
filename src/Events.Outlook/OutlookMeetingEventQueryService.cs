using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using DataSources.Outlook;
using Graph;

namespace Events.Outlook
{
    public class OutlookMeetingEventQueryService : IEventQueryService
    {
        private readonly IOutlookDataProvider m_outlookDataSource;

        public OutlookMeetingEventQueryService()
        {
            m_outlookDataSource = new OutlookDataProvider();
        }

        public IEnumerable<Event> PullEvents(DateTime startDateTime, DateTime stopDateTime)
        {
            var meetings = m_outlookDataSource.GetMeetings(startDateTime, stopDateTime);

            return meetings.Select(x =>
                new Event()
                {
                    Text = x.Body ?? string.Empty,
                    Subject = new Subject() { Text = x.Subject },
                    Date = x.StartUtc,
                    Duration = TimeSpan.FromMinutes(x.Duration.TotalMinutes),
                    Participants =
                        x.Recipients.Select(y => new Participant(y)).ToGraph()
                });
        }
    }
}
