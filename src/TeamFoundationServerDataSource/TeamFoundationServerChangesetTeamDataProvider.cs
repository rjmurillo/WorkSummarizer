using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Microsoft.TeamFoundation.VersionControl.Client;


namespace DataSources.TeamFoundationServer
{
    public class TeamFoundationServerChangesetTeamDataProvider
    {
        private readonly IWorkItemDataPull m_data;
        private readonly TeamFoundationServerWorkItemTeamDataProvider m_workItemTeamDataProvider;

        public TeamFoundationServerChangesetTeamDataProvider(
            IWorkItemDataPull data,
            TeamFoundationServerWorkItemTeamDataProvider workItemTeamDataProvider
            )
        {
            m_data = data;
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
            var wi = m_data.PullWorkItems(wid);
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