using Extensibility;

namespace Renders.HTML
{
    public class HtmlRenderSettingConstants
    {
        public const string OutputDirectory = "HTML.OutputDirectory";
    }

    public class HtmlRenderPlugin
    {
        public HtmlRenderPlugin(IPluginContext pluginContext)
        {
            pluginContext.RegisterService<IRenderEvents>(new HtmlWriteEvents(), new ServiceRegistration("HTML", "HTML", "File"));
        }
    }
}
