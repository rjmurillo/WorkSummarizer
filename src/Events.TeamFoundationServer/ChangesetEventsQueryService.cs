using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using DataSources.TeamFoundationServer;
using DataSources.Who;
using WorkSummarizer.TeamFoundationServerDataSource;

namespace Events.TeamFoundationServer
{
    public class ChangesetEventsQueryService : EventQueryServiceBase
    {
        private readonly Uri m_teamFoundationServer;
        private readonly string m_project;

        public ChangesetEventsQueryService(Uri teamFoundationServerUri, string projectName)
        {
            m_teamFoundationServer = teamFoundationServerUri;
            m_project = projectName;
        }

        public override IEnumerable<Event> PullEvents(DateTime startDateTime, DateTime stopDateTime, Func<Event, bool> predicate)
        {
            var source = new TeamFoundationServerChangesetDataProvider(m_teamFoundationServer, m_project);
            var data = source.PullData(startDateTime, stopDateTime);

            var retval = data.Select(
                p =>
                {
                    var e = new Event
                    {
                        Text = p.Comment,
                        Date = p.CreationDate,
                        Duration = TimeSpan.Zero,
                        Context = p.ChangesetId
                    };
                    e.Participants.Add(IdentityUtility.Create(p.Committer));
                    return e;
                });

            return (predicate != null) ? retval.Where(predicate) : retval;
        }
    }
}
