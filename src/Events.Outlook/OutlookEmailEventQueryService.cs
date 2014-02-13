using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using DataSources;
using DataSources.Outlook;
using Graph;

namespace Events.Outlook
{
    public class OutlookEmailEventQueryService : IEventQueryService
    {
        private readonly IDataPull<OutlookItem> m_outlookDataSource;

        public OutlookEmailEventQueryService()
        {
            m_outlookDataSource = new OutlookDataEmailProvider();
        }

        public IEnumerable<Event> PullEvents(DateTime startDateTime, DateTime stopDateTime)
        {
            var meetings = m_outlookDataSource.PullData(startDateTime, stopDateTime);

            return meetings.Select(x =>
                new Event()
                {
                    Text = x.Body,
                    Subject = new Subject() { Text = x.Subject },
                    Date = x.StartUtc,
                    Duration = TimeSpan.FromMinutes(x.Duration.TotalMinutes),
                    Participants =
                        x.Recipients.Select(y => new Participant(y)).ToGraph()
                });
        }
    }
}