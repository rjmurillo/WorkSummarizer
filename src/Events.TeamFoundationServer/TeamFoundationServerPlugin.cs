using System;
using Extensibility;

namespace Events.TeamFoundationServer
{
    public class TeamFoundationServerPlugin
    {
        public TeamFoundationServerPlugin(IPluginRuntime pluginRuntime)
        {
            var workItemConfigurationService = new DefaultConfigurationService(new[]
            {
                new ConfigurationSetting("TeamFoundationService.Changesets.SkipWhenHistoryContains", "TFS AUTO UPDATE") 
                { 
                    Name = "Skip when history contains", 
                    Description = "Skip processing the event when the work item history contains this value." 
                }
            });

            pluginRuntime.ConfigurationServices[new ServiceRegistration("TeamFoundationService.Changesets", "Team Foundation Server", "Configure Work Items")] = workItemConfigurationService; // REVIEW not depend on configuration service id to line up with another service

            pluginRuntime.EventQueryServices[new ServiceRegistration("TeamFoundationService.Changesets", "Team Foundation Server", "Changesets")] = new ChangesetEventsQueryService();
            pluginRuntime.EventQueryServices[new ServiceRegistration("TeamFoundationService.WorkItems", "Team Foundation Server", "Work Items")] = new WorkItemEventsQueryService(); // REVIEW from configuration service pulled from plugin runtime            
        }
    }
}
