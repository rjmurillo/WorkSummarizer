using Extensibility;

namespace Events.Connect
{
    public class ConnectPlugin
    {
        public ConnectPlugin(IPluginRuntime pluginRuntime)
        {
            pluginRuntime.EventQueryServices["Connect.History"] = new ConnectHistoryEventQueryService();
        }
    }
}
