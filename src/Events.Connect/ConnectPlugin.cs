using Extensibility;

namespace Events.Connect
{
    public class ConnectPlugin
    {
        public ConnectPlugin(IPluginRuntime pluginRuntime)
        {
            pluginRuntime.EventQueryServices[new ServiceRegistration("Connect.History", "Connect", "History") { InvokeOnShellDispatcher = true }] = new ConnectHistoryEventQueryService();
        }
    }
}
