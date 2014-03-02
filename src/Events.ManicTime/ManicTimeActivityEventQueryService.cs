using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using DataSources.ManicTime;
using DataSources.Who;
using Extensibility;

namespace Events.ManicTime
{
    public class ManicTimeActivityEventQueryService : EventQueryServiceBase
    {
        private readonly IConfigurationService m_configurationService;

        public ManicTimeActivityEventQueryService(IConfigurationService configurationService)
        {
            m_configurationService = configurationService;
        }

        public override IEnumerable<Event> PullEvents(DateTime startDateTime, DateTime stopDateTime, Func<Event,bool> predicate)
        {
            var m = new ManicTimeDataProvider(m_configurationService.GetValueOrDefault(ManicTimeSettingConstants.ActivitiesDatabaseFile));

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
