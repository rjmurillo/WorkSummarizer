﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using DataSources;
using DataSources.Outlook;
using DataSources.Who;
using Graph;

namespace Events.Outlook
{
    public class OutlookMeetingEventQueryService : EventQueryServiceBase
    {
        private readonly IDataPull<OutlookItem> m_outlookDataSource;

        public OutlookMeetingEventQueryService()
        {
            m_outlookDataSource = new MeetingProvider();
        }

        public override IEnumerable<Event> PullEvents(DateTime startDateTime, DateTime stopDateTime, Func<Event, bool> predicate)
        {
            var meetings = m_outlookDataSource.PullData(startDateTime, stopDateTime);

            var retval = meetings.Select(x =>
                new Event()
                {
                    Text = x.Body ?? string.Empty,
                    Subject = new Subject() { Text = x.Subject },
                    Date = x.StartUtc,
                    Duration = TimeSpan.FromMinutes(x.Duration.TotalMinutes),
                    Participants =
                        x.Recipients.Select(IdentityUtility.Create).ToGraph()
                });

            return (predicate != null) ? retval.Where(predicate) : retval;
        }
    }
}
