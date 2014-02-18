using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using DataSources.TeamFoundationServer;
using DataSources.Who;
using Graph;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using WorkSummarizer.TeamFoundationServerDataSource;

namespace Events.TeamFoundationServer
{
    public class WorkItemEventsQueryServiceInternal : TeamFoundationServerEventQueryServiceBase
    {
        public WorkItemEventsQueryServiceInternal(Uri teamFoundationServerUri, string projectName)
            :base(teamFoundationServerUri, projectName)
        {
        }

        public override IEnumerable<Event> PullEvents(DateTime startDateTime, DateTime endDateTime, Func<Event, bool> predicate)
        {
            var source = new TeamFoundationServerWorkItemDataProvider(TeamFoundationServer, Project);
            var wis = source.PullData(startDateTime, endDateTime);

            // Convert WI to Event
            var retval = new List<Event>();
            foreach (var wi in wis)
            {
                foreach (Revision r in wi.Revisions)
                {
                    var e = new Event();

                    e.Date = (DateTime)r.Fields["Changed date"].Value;
                    e.Duration = TimeSpan.Zero;
                    e.Text = (string)r.Fields["History"].Value;
                    e.EventType = r.Index == 0 ? "TeamFoundationServer.WorkItem.Created" : "TeamFoundationServer.WorkItem.Revision";

                    if (r.Fields.Contains("State"))
                    {
                        var stateValue = r.Fields["State"].Value as string;
                        if (!string.IsNullOrWhiteSpace(stateValue))
                        {
                            if (stateValue.Equals("Resolved", StringComparison.OrdinalIgnoreCase))
                            {
                                e.EventType = "TeamFoundationServer.WorkItem.Resolved";
                            }
                            else if (stateValue.Equals("Closed", StringComparison.OrdinalIgnoreCase))
                            {
                                e.EventType = "TeamFoundationServer.WorkItem.Closed";
                            }
                            else if (stateValue.Equals("Active", StringComparison.OrdinalIgnoreCase))
                            {
                                e.EventType = "TeamFoundationServer.WorkItem.Activated";
                            }
                        }
                    }

                    e.Context = wi.Id;

                    if (e.Text.Contains("TFS AUTO UPDATE"))
                    {
                        continue;
                    }

                    if (e.Date.Ticks > startDateTime.Ticks && e.Date.Ticks < endDateTime.Ticks)
                    {
                        var p = IdentityUtility.Create((string)r.Fields["Changed by"].Value);
                        e.Participants = new Graph<Participant> { p };

                        retval.Add(e);
                    }
                }
            }

            return (predicate != null) ? retval.Where(predicate) : retval;
        }
    }
}
