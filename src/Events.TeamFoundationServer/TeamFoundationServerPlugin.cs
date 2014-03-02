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
        
        public TeamFoundationServerPlugin(IPluginRuntime pluginRuntime)
        {
            var workItemConfigurationService = new DefaultConfigurationService(new[]
            {
                new ConfigurationSetting(TeamFoundationServerSettingConstants.WorkItemSkipWhenHistoryContains, "TFS AUTO UPDATE") 
                { 
                    Name = "Skip When History Contains Value", 
                    Description = "Do not process the event when the work item history contains this value." 
                }
            });

            pluginRuntime.ConfigurationServices[new ServiceRegistration("TeamFoundationService.WorkItems", "Team Foundation Server", "Work Items")] = workItemConfigurationService; // REVIEW not depend on configuration service id to line up with another service

            pluginRuntime.EventQueryServices[new ServiceRegistration("TeamFoundationService.Changesets", "Team Foundation Server", "Changesets")] = new ChangesetEventsQueryService();
            pluginRuntime.EventQueryServices[new ServiceRegistration("TeamFoundationService.WorkItems", "Team Foundation Server", "Work Items") { IsConfigurable = true }] = new WorkItemEventsQueryService(workItemConfigurationService); // REVIEW from configuration service pulled from plugin runtime            
        }
    }
}
