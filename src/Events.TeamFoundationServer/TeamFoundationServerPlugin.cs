using System;
using Extensibility;

namespace Events.TeamFoundationServer
{
    public class TeamFoundationServerPlugin
    {
        public TeamFoundationServerPlugin(IPluginRuntime pluginRuntime)
        {
            pluginRuntime.EventQueryServices[new ServiceRegistration("TeamFoundationService.Changesets", "Team Foundation Server", "Changesets")] = new ChangesetEventsQueryService();
            pluginRuntime.EventQueryServices[new ServiceRegistration("TeamFoundationService.WorkItems", "Team Foundation Server", "Work Items")] = new WorkItemEventsQueryService(); // REVIEW from configuration service pulled from plugin runtime
        }
    }
}
