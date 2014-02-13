using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extensibility;

namespace Events.Kudos
{
    public class KudosPlugin
    {
        public KudosPlugin(IPluginRuntime pluginRuntime)
        {
            pluginRuntime.EventQueryServices["Kudos.Received"] = new KudosReceivedEventQueryService();
        }
    }
}
