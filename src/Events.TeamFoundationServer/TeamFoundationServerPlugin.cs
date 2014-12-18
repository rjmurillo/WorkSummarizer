using System;
using Extensibility;

namespace Events.TeamFoundationServer
{
    public class TeamFoundationServerSettingConstants
    {
        public const string WorkItemSkipWhenHistoryContains = "TeamFoundationService.Changesets.SkipWhenHistoryContains";
    }

    public class TeamFoundationServerPlugin
    {
        
        public TeamFoundationServerPlugin(IPluginContext pluginContext)
        {
            pluginContext.RegisterService<IEventQueryService>(new ChangesetEventsQueryService(), new ServiceRegistration("TeamFoundationService.Changesets", "Team Foundation Server", "Changesets"));
            pluginContext.RegisterService<IEventQueryService>(new WorkItemEventsQueryService(), new ServiceRegistration("TeamFoundationService.WorkItems", "Team Foundation Server", "Work Items"));
        }
    }
}
