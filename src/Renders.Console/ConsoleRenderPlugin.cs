using Extensibility;

namespace Renders.Console
{
    public class ConsoleRenderPlugin
    {
        public ConsoleRenderPlugin(IPluginContext pluginContext)
        {
            pluginContext.RegisterService<IRenderEvents>(new ConsoleWriteEvents(), new ServiceRegistration("Console", "Console", "Console"));
        }
    }
}
