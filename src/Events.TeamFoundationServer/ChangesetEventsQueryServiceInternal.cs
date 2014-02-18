using System;
using System.Collections.Generic;
using System.Linq;
using DataSources.TeamFoundationServer;
using DataSources.Who;

namespace Events.TeamFoundationServer
{
    internal class ChangesetEventsQueryServiceInternal : TeamFoundationServerEventQueryServiceBase
    {
        public ChangesetEventsQueryServiceInternal(Uri teamFoundationServerUri, string projectName)
            : base(teamFoundationServerUri, projectName)
        {
        }

        public override IEnumerable<Event> PullEvents(DateTime startDateTime, DateTime stopDateTime, Func<Event, bool> predicate)
        {
            var source = new TeamFoundationServerChangesetDataProvider(TeamFoundationServer, Project);
            var data = source.PullData(startDateTime, stopDateTime);

            var retval = data.Select(
                p =>
                {
                    var e = new Event
                    {
                        Text = p.Comment,
                        Date = p.CreationDate,
                        Duration = TimeSpan.Zero,
                        Context = p.ChangesetId,
                        EventType = "TeamFoundationServer.Changeset"
                    };
                    e.Participants.Add(IdentityUtility.Create(p.Committer));
                    return e;
                }).ToList();

            return (predicate != null) ? retval.Where(predicate) : retval;
        }
    }
}