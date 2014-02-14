using Extensibility;

namespace Renders.Excel
{
    public class ExcelRenderPlugin
    {
        public ExcelRenderPlugin(IPluginRuntime pluginRuntime)
        {
            pluginRuntime.RenderEventServices[new ServiceRegistration("Excel", "Excel", "Spreadsheet")] = new ExcelWriteEvents();
        }
    }
}
