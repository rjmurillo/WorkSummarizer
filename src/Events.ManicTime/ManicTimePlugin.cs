using Extensibility;

namespace Events.ManicTime
{
    public class ManicTimePlugin
    {
        public ManicTimePlugin(IPluginRuntime pluginRuntime)
        {
            pluginRuntime.EventQueryServices[new ServiceRegistration("ManicTime.Activites", "ManicTime", "Activities")] = new ManicTimeActivityEventQueryService();
        }
    }
}
