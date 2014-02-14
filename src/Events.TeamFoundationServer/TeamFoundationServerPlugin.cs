using System;
using Extensibility;

namespace Events.TeamFoundationServer
{
    public class TeamFoundationServerPlugin
    {
        public TeamFoundationServerPlugin(IPluginRuntime pluginRuntime)
        {
            pluginRuntime.EventQueryServices[new ServiceRegistration("TeamFoundationService.Changesets", "Team Foundation Server", "Changesets")] = new ChangesetEventsQueryService(new Uri("http://vstfcodebox:8080/tfs/Engineering_Excellence"), "EE_Engineering");
            pluginRuntime.EventQueryServices[new ServiceRegistration("TeamFoundationService.WorkItems", "Team Foundation Server", "Work Items")] = new WorkItemEventsQueryService(new Uri("http://vstfcodebox:8080/tfs/Engineering_Excellence"), "EE_Engineering"); // REVIEW from configuration service pulled from plugin runtime
        }
    }
}
