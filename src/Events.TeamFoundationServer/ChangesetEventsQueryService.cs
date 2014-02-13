using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using DataSources.TeamFoundationServer;
using WorkSummarizer.TeamFoundationServerDataSource;

namespace Events.TeamFoundationServer
{
    public class ChangesetEventsQueryService : IEventQueryService
    {
        private readonly Uri m_teamFoundationServer;
        private readonly string m_project;

        public ChangesetEventsQueryService(Uri teamFoundationServerUri, string projectName)
        {
            m_teamFoundationServer = teamFoundationServerUri;
            m_project = projectName;
        }

        public IEnumerable<Event> PullEvents(DateTime startDateTime, DateTime stopDateTime)
        {
            var source = new TeamFoundationServerChangesetDataProvider(m_teamFoundationServer, m_project);
            return source.PullData(startDateTime, stopDateTime).Select(
                p =>
                {
                    var e = new Event
                    {
                        Text = p.Comment,
                        Date = p.CreationDate,
                        Duration = TimeSpan.Zero,
                        Context = p.ChangesetId
                    };
                    e.Participants.Add(new Participant(p.Committer));
                    return e;
                });
        }
    }
}
