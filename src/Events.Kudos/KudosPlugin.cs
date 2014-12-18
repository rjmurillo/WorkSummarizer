using Extensibility;

namespace Events.Kudos
{
    public class KudosPlugin
    {
        public KudosPlugin(IPluginContext pluginContext)
        {
            pluginContext.RegisterService<IEventQueryService>(new KudosReceivedEventQueryService(), new ServiceRegistration("Kudos.Received", "Kudos", "Received"));
        }
    }
}
