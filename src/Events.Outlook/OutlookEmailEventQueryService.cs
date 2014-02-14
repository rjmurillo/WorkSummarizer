using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using DataSources;
using DataSources.Outlook;
using DataSources.Who;
using Graph;

namespace Events.Outlook
{
    public class OutlookEmailEventQueryService : EventQueryServiceBase
    {
        private readonly IDataPull<OutlookItem> m_outlookDataSource;

        public OutlookEmailEventQueryService()
        {
            m_outlookDataSource = new EmailProvider();
        }

        public override IEnumerable<Event> PullEvents(DateTime startDateTime, DateTime stopDateTime, Func<Event, bool> predicate )
        {
            var meetings = m_outlookDataSource.PullData(startDateTime, stopDateTime);

            var retval = meetings.Select(x =>
                new Event()
                {
                    Text = String.Format("{0} {1}", x.Subject ?? String.Empty, x.Body ?? string.Empty),
                    Date = x.StartUtc,
                    Duration = TimeSpan.FromMinutes(x.Duration.TotalMinutes),
                    Participants =
                        x.Recipients.Select(IdentityUtility.Create).ToGraph()
                });

            return (predicate != null) ? retval.Where(predicate) : retval;
        }
    }
}