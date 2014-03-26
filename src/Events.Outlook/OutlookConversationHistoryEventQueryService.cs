using System;
using System.Collections.Generic;
using System.Linq;
using DataSources;
using DataSources.Outlook;
using DataSources.Who;
using Graph;

namespace Events.Outlook
{
    using Extensibility;

    public class OutlookConversationHistoryEventQueryService : EventQueryServiceBase, IConfigurable
    {
        private readonly IDataPull<OutlookItem> m_outlookDataSource;
        private readonly IDictionary<string, ConfigurationSetting> m_settings;

        private readonly ConfigurationSetting m_includeOnlyConversationHistoryAliasesSetting;

        public OutlookConversationHistoryEventQueryService()
        {
            m_outlookDataSource = new ConversationHistoryProvider();

            m_includeOnlyConversationHistoryAliasesSetting = new ConfigurationSetting(OutlookSettingConstants.IncludeOnlyMeetingAliases, string.Empty)
            {
                Name = "Include Only From",
                Description = "Conversation history not matching these comma-separated aliases will be excluded."
            };

            m_settings = new[]
                         {
                            m_includeOnlyConversationHistoryAliasesSetting
                         }.ToDictionary(p => p.Key);
        }

        public IEnumerable<ConfigurationSetting> Settings { get { return m_settings.Values; } }

        public override IEnumerable<Event> PullEvents(DateTime startDateTime, DateTime stopDateTime, Func<Event, bool> predicate)
        {
            var meetings = m_outlookDataSource.PullData(startDateTime, stopDateTime);

            var retval = meetings.Select(x =>
                new Event()
                {
                    EventType = "Outlook.ConversationHistory",
                    Text = String.Format("{0} {1}", x.Subject ?? String.Empty, x.Body ?? string.Empty),
                    Date = x.StartUtc,
                    Duration = TimeSpan.FromMinutes(x.Duration.TotalMinutes),
                    Participants =
                        x.Recipients.Select(IdentityUtility.Create).ToGraph()
                }).ToList();

            var includeOnlyAliases = new HashSet<string>(m_includeOnlyConversationHistoryAliasesSetting.Value.Split(new []{','}, StringSplitOptions.RemoveEmptyEntries), StringComparer.OrdinalIgnoreCase);
            if (includeOnlyAliases.Any())
            {
                retval = retval.Where(p => p.Participants.Any(q => includeOnlyAliases.Contains(q.Value.Alias))).ToList();
            }

            return (predicate != null) ? retval.Where(predicate) : retval;
        }
    }
}