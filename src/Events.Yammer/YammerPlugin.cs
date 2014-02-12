using Extensibility;

namespace Events.Yammer
{
    public class YammerPlugin
    {
        public YammerPlugin(IPluginRuntime pluginRuntime)
        {
            pluginRuntime.EventQueryServices["Yammer.SentMessages"] = new YammerEventsQueryService();
        }
    }
}
