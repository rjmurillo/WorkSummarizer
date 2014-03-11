using System;
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
    using Extensibility;

    public class OutlookMeetingEventQueryService : EventQueryServiceBase, IConfigurable
    {
        private readonly IDataPull<OutlookItem> m_outlookDataSource;
        private readonly IDictionary<string, ConfigurationSetting> m_settings;

        private readonly ConfigurationSetting m_includeOnlyMeetingAliasesSetting;

        public OutlookMeetingEventQueryService()
        {
            m_outlookDataSource = new MeetingProvider();

            m_includeOnlyMeetingAliasesSetting = new ConfigurationSetting(OutlookSettingConstants.IncludeOnlyMeetingAliases, string.Empty)
            {
                Name = "Include Only From",
                Description = "Meetings not matching these comma-separated aliases will be excluded."
            };

            m_settings = new[]
                         {
                            m_includeOnlyMeetingAliasesSetting
                         }.ToDictionary(p => p.Key);
        }

        public IEnumerable<ConfigurationSetting> Settings { get { return m_settings.Values; } }

        public override IEnumerable<Event> PullEvents(DateTime startDateTime, DateTime stopDateTime, Func<Event, bool> predicate)
        {
            var meetings = m_outlookDataSource.PullData(startDateTime, stopDateTime);

            var retval = meetings.Select(x =>
                new Event()
                {
                    Text = String.Format("{0} {1}", x.Subject ?? String.Empty, x.Body ?? string.Empty),
                    Date = x.StartUtc,
                    Duration = TimeSpan.FromMinutes(x.Duration.TotalMinutes),
                    EventType = "Outlook.Meeting",
                    Participants =
                        x.Recipients.Select(IdentityUtility.Create).ToGraph()
                }).ToList();

            var includeOnlyAliases = new HashSet<string>(m_includeOnlyMeetingAliasesSetting.Value.Split(','), StringComparer.OrdinalIgnoreCase);
            if (includeOnlyAliases.Any())
            {
                retval = retval.Where(p => p.Participants.Any(q => includeOnlyAliases.Contains(q.Value.Alias))).ToList();
            }

            return (predicate != null) ? retval.Where(predicate) : retval;
        }
    }
}
