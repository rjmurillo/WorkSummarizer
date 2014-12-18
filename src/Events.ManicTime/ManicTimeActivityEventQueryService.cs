using System;
using System.Collections.Generic;
using System.Linq;
using DataSources.ManicTime;
using DataSources.Who;
using Extensibility;

namespace Events.ManicTime
{
    using System.IO;

    public class ManicTimeActivityEventQueryService : EventQueryServiceBase, IConfigurable
    {
        private readonly IDictionary<string, ConfigurationSetting> m_settings;

        public ManicTimeActivityEventQueryService()
        {
            var manicTimeDbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "..\\Local\\Finkit\\ManicTime");
            if (!Directory.Exists(manicTimeDbPath))
            {
                manicTimeDbPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            }

            var manicTimeDbFile = Path.Combine(manicTimeDbPath, "ManicTime.sdf");
            m_settings = new[]
            {
                new ConfigurationSetting(ManicTimeSettingConstants.ActivitiesDatabaseFile, manicTimeDbFile) 
                { 
                    Name = "Database File Location", 
                    Description = "Path to the *.sdf database." 
                }
            }.ToDictionary(p => p.Key);
        }

        public IEnumerable<ConfigurationSetting> Settings { get { return m_settings.Values; } }

        public override IEnumerable<Event> PullEvents(DateTime startDateTime, DateTime stopDateTime, Func<Event,bool> predicate)
        {
            var m = new ManicTimeDataProvider(m_settings[ManicTimeSettingConstants.ActivitiesDatabaseFile].Value);

            var retval = m.PullData(startDateTime, stopDateTime).Select(p => 
            {
                var e = new Event
                {
                    Date = p.StartUtcTime,
                    Duration = p.EndUtcTime - p.StartUtcTime,
                    EventType = "ManicTime.Activity",
                    Context = p.DisplayName,
                    Text = p.GroupDisplayName
                };

                // "Utilities" and other "helpers" should be injected via the plugin runtime
                e.Participants.Add(IdentityUtility.Create(Environment.UserName));

                return e;
            }).ToList();

            return (predicate != null) ? retval.Where(predicate) : retval;
        }
    }
}
