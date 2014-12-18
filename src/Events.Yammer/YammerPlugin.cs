using Extensibility;

namespace Events.Yammer
{
    public class YammerPlugin
    {
        public YammerPlugin(IPluginContext pluginContext)
        {
            pluginContext.RegisterService<IEventQueryService>(new YammerEventsQueryService(), new ServiceRegistration("Yammer.SentMessages", "Yammer", "Sent messages") { InvokeOnShellDispatcher = true });
        }
    }
}
