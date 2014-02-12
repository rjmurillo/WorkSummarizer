using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Events;
using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Graph;
using WorkSummarizer.TeamFoundationServerDataSource;

namespace TfsCodeSwarm
{
    public class WorkItemEventsQueryService : IEventQueryService
    {
        public WorkItemEventsQueryService(Uri teamFoundationServerUri, string projectName)
        {
            TeamFoundationServer = teamFoundationServerUri;
            Project = projectName;
        }

        public Uri TeamFoundationServer { get; private set; }
        public string Project { get; private set; }

        public IEnumerable<Event> PullEvents(DateTime startDateTime, DateTime endDateTime)
        {
            var source = new TfsData();
            var wis = source.PullWorkItemsThatChanged(TeamFoundationServer, Project, startDateTime, endDateTime);

            // Convert WI to Event
            var retval = new List<Event>();
            foreach (var wi in wis)
            {
                foreach (Revision r in wi.Revisions)
                {
                    var e = new Event();
                    var p = new Participant((string)r.Fields["Changed by"].Value);

                    e.Participants = new Graph<Participant> { p };
                    e.Date = (DateTime)r.Fields["Changed date"].Value;
                    e.Duration = 0d;
                    e.Text = (string)r.Fields["History"].Value;
                    e.EventType = r.Index == 0 ? "TFS Workitem Created" : "TFS Workitem Revision";

                    if (r.Fields.Contains("State"))
                    {
                        var stateValue = r.Fields["State"].Value as string;
                        if (!string.IsNullOrWhiteSpace(stateValue))
                        {
                            if (stateValue.Equals("Resolved", StringComparison.OrdinalIgnoreCase))
                            {
                                e.EventType = "TFS Workitem Resolve";
                            }
                            else if (stateValue.Equals("Closed", StringComparison.OrdinalIgnoreCase))
                            {
                                e.EventType = "TFS Workitem Closed";
                            }
                            else if (stateValue.Equals("Active", StringComparison.OrdinalIgnoreCase))
                            {
                                e.EventType = "TFS Workitem Activated";
                            }
                        }
                    }

                    e.Context = wi.Id;
                    retval.Add(e);
                }
            }

            return retval;
        }
    }
}
