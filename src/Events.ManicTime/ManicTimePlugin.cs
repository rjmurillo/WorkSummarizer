using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extensibility;

namespace Events.ManicTime
{
    public class ManicTimePlugin
    {
        public ManicTimePlugin(IPluginRuntime pluginRuntime)
        {
            pluginRuntime.EventQueryServices["ManicTime.Activites"] = new ManicTimeActivityEventQueryService();
        }
    }
}
