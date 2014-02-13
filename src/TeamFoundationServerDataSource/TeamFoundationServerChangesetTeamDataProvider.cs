using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Microsoft.TeamFoundation.VersionControl.Client;
using WorkSummarizer.TeamFoundationServerDataSource;

namespace DataSources.Team
{
    public class TeamFoundationServerChangesetTeamDataProvider
    {
        private readonly ITfsData m_data;
        private readonly Uri m_teamFoundationServerUri;
        private readonly string m_projectName;
        private readonly TeamFoundationServerWorkItemTeamDataProvider m_workItemTeamDataProvider;

        public TeamFoundationServerChangesetTeamDataProvider(
            ITfsData data, 
            Uri teamFoundationServerUri, 
            string projectName,
            TeamFoundationServerWorkItemTeamDataProvider workItemTeamDataProvider
            )
        {
            m_data = data;
            m_teamFoundationServerUri = teamFoundationServerUri;
            m_projectName = projectName;
            m_workItemTeamDataProvider = workItemTeamDataProvider;
        }

        public IEnumerable<Participant> InferParticipantsFromChangesets(IEnumerable<Changeset> changesets)
        {
            return InferParticipantsFromChangesets(changesets, null);
        }

        public IEnumerable<Participant> InferParticipantsFromChangesets(IEnumerable<Changeset> changesets,
            Func<Changeset, bool> predicate)
        {
            var wid = PullWorkItemIdentitiesFromChangesets(changesets, predicate);
            var wi = m_data.PullWorkItems(m_teamFoundationServerUri, m_projectName, wid);
            return m_workItemTeamDataProvider.InferParticipantsFromWorkItems(wi);
        }

        private IEnumerable<int> PullWorkItemIdentitiesFromChangesets(IEnumerable<Changeset> changesets, Func<Changeset, bool> predicate)
        {
            List<int> workItemsIdentities = new List<int>();

            if (predicate != null)
            {
                changesets = changesets.Where(predicate);
            }

            foreach (Changeset changeset in changesets)
            {
                var workItems = changeset.AssociatedWorkItems;
                if (workItems != null)
                {
                    workItemsIdentities.AddRange(workItems.Select(s => s.Id));
                }
            }

            return workItemsIdentities.Distinct();
        }
    }
}