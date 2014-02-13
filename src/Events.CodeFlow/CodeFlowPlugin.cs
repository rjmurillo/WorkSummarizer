using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extensibility;

namespace Events.CodeFlow
{
    public class CodeFlowPlugin
    {
        public CodeFlowPlugin(IPluginRuntime pluginRuntime)
        {
            pluginRuntime.EventQueryServices["CodeFlow.Authored"] = new CodeFlowAuthoredEventQueryService();
        }
    }
}
