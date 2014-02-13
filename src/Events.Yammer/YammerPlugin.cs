using Extensibility;

namespace Events.Yammer
{
    public class YammerPlugin
    {
        public YammerPlugin(IPluginRuntime pluginRuntime)
        {
            pluginRuntime.EventQueryServices[new ServiceRegistration("Yammer.SentMessages", "Yammer", "Sent messages")] = new YammerEventsQueryService();
        }
    }
}
