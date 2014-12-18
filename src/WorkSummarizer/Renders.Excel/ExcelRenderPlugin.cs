using Extensibility;

namespace Renders.Excel
{
    public class ExcelRenderPlugin
    {
        public ExcelRenderPlugin(IPluginContext pluginContext)
        {
            pluginContext.RegisterService<IRenderEvents>(new ExcelWriteEvents(), new ServiceRegistration("Excel", "Excel", "Spreadsheet"));
        }
    }
}
