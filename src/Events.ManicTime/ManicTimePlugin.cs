using Extensibility;

namespace Events.ManicTime
{
    public class ManicTimeSettingConstants
    {
        public const string ActivitiesDatabaseFile = "ManicTime.Activities.DatabaseFile";
    }

    public class ManicTimePlugin
    {
        public ManicTimePlugin(IPluginContext pluginContext)
        {
            pluginContext.RegisterService<IEventQueryService>(new ManicTimeActivityEventQueryService(), new ServiceRegistration("ManicTime.Activities", "ManicTime", "Activities"));
        }
    }
}
