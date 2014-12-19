using Extensibility;

namespace Events.Connect
{
    public class ConnectPlugin
    {
        public ConnectPlugin(IPluginContext pluginContext)
        {
            pluginContext.RegisterService<IEventQueryService>(new ConnectHistoryEventQueryService(), new ServiceRegistration("Connect.History", "Connect", "History") { InvokeOnShellDispatcher = true });
        }
    }
}
