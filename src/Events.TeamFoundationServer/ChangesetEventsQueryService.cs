using System;
using System.Collections.Generic;
using System.Linq;
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
            var source = new TfsData();
            return source.PullChangesets(m_teamFoundationServer, m_project, startDateTime, stopDateTime).Select(
                p => 
                { 
                    return new Event{ Text = p.Comment, Date = p.CreationDate }; 
                });
        }
    }
}
