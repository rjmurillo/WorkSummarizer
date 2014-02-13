﻿using System;
using System.Collections.Generic;
using Common;
using Graph;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using WorkSummarizer.TeamFoundationServerDataSource;

namespace Events.TeamFoundationServer
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
            var source = new TeamFoundationServerDataProvider();
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
                    e.Duration = TimeSpan.Zero;
                    e.Text = (string)r.Fields["History"].Value;
                    e.EventType = r.Index == 0 ? "TFS Work item Created" : "TFS Work item Revision";

                    if (r.Fields.Contains("State"))
                    {
                        var stateValue = r.Fields["State"].Value as string;
                        if (!string.IsNullOrWhiteSpace(stateValue))
                        {
                            if (stateValue.Equals("Resolved", StringComparison.OrdinalIgnoreCase))
                            {
                                e.EventType = "TFS Work item Resolve";
                            }
                            else if (stateValue.Equals("Closed", StringComparison.OrdinalIgnoreCase))
                            {
                                e.EventType = "TFS Work item Closed";
                            }
                            else if (stateValue.Equals("Active", StringComparison.OrdinalIgnoreCase))
                            {
                                e.EventType = "TFS Work item Activated";
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
