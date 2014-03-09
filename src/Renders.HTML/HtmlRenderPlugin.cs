using Extensibility;

namespace Renders.HTML
{
    public class HtmlRenderSettingConstants
    {
        public const string OutputDirectory = "HTML.OutputDirectory";
        public const string FloorHeatmapOutputDirectory = "HTML.FloorHeatmap.OutputDirectory";
    }

    public class HtmlRenderPlugin
    {
        public HtmlRenderPlugin(IPluginContext pluginContext)
        {
            pluginContext.RegisterService<IRenderEvents>(new HtmlWriteEvents(), new ServiceRegistration("HTML.TagCloud", "HTML", "Tag Cloud"));
            pluginContext.RegisterService<IRenderEvents>(new HtmlFloorHeatmapRenderEvents(), new ServiceRegistration("HTML.FloorHeatmap", "HTML", "Floor Heatmap"));
        }
    }
}
