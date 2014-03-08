using Extensibility;

namespace Events.CodeFlow
{
    public class CodeFlowPlugin
    {
        public CodeFlowPlugin(IPluginContext pluginContext)
        {
            pluginContext.RegisterService<IEventQueryService>(new CodeFlowAuthoredEventQueryService(), new ServiceRegistration("CodeFlow.Authored", "CodeFlow", "Authored"));
        }
    }
}
