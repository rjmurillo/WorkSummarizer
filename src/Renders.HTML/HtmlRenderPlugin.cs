using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extensibility;

namespace Renders.HTML
{
    public class HtmlRenderPlugin
    {
        public HtmlRenderPlugin(IPluginRuntime pluginRuntime)
        {
            pluginRuntime.RenderEventServices[new ServiceRegistration("HTML", "HTML", "File")] = new HtmlWriteEvents();
        }
    }
}
