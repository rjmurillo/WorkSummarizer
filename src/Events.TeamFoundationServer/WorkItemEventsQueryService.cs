using System;
using System.Collections.Generic;
using DataSources.TeamFoundationServer;
using Extensibility;

namespace Events.TeamFoundationServer
{
    using System.Linq;

    public class WorkItemEventsQueryService : IEventQueryService, IConfigurable
    {
        private readonly IDictionary<string, ConfigurationSetting> m_settings;

        public WorkItemEventsQueryService()
        {
            m_settings = new[]
            {
                new ConfigurationSetting(TeamFoundationServerSettingConstants.WorkItemSkipWhenHistoryContains,
                    "TFS AUTO UPDATE")
                {
                    Name = "Skip When History Contains Value",
                    Description = "Do not process the event when the work item history contains this value."
                }
            }.ToDictionary(p => p.Key);
        }

        public IEnumerable<ConfigurationSetting> Settings { get { return m_settings.Values; } }

        public IEnumerable<Event> PullEvents(DateTime startDateTime, DateTime stopDateTime)
        {
            List<Event> retval = new List<Event>();

            var servers = TeamFoundationServerRegistrationUtility.LoadRegisteredTeamFoundationServers();

            foreach (var server in servers)
            {
                var q = new WorkItemEventsQueryServiceInternal(server, null, m_settings[TeamFoundationServerSettingConstants.WorkItemSkipWhenHistoryContains].Value);
                retval.AddRange(q.PullEvents(startDateTime, stopDateTime));
            }

            return retval;
        }

        public IEnumerable<Event> PullEvents(DateTime startDateTime, DateTime stopDateTime, string alias)
        {
            List<Event> retval = new List<Event>();

            var servers = TeamFoundationServerRegistrationUtility.LoadRegisteredTeamFoundationServers();

            foreach (var server in servers)
            {
                var q = new WorkItemEventsQueryServiceInternal(server, null, m_settings[TeamFoundationServerSettingConstants.WorkItemSkipWhenHistoryContains].Value);
                retval.AddRange(q.PullEvents(startDateTime, stopDateTime, alias));
            }

            return retval;
        }

        public IEnumerable<Event> PullEvents(DateTime startDateTime, DateTime stopDateTime, Func<Event, bool> predicate)
        {
            List<Event> retval = new List<Event>();

            var servers = TeamFoundationServerRegistrationUtility.LoadRegisteredTeamFoundationServers();

            foreach (var server in servers)
            {
                var q = new WorkItemEventsQueryServiceInternal(server, null, m_settings[TeamFoundationServerSettingConstants.WorkItemSkipWhenHistoryContains].Value);
                retval.AddRange(q.PullEvents(startDateTime, stopDateTime, predicate));
            }

            return retval;
        }
    }
}