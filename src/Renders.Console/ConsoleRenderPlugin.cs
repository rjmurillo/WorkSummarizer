using Extensibility;

namespace Renders.Console
{
    public class ConsoleRenderPlugin
    {
        public ConsoleRenderPlugin(IPluginRuntime pluginRuntime)
        {
            pluginRuntime.RenderEventServices[new ServiceRegistration("Console", "Console", "Console")] = new ConsoleWriteEvents();
        }
    }
}
