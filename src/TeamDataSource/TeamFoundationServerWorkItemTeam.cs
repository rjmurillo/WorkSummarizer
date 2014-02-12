using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using WorkSummarizer.TeamDataSource;
using WorkSummarizer.TeamFoundationServerDataSource;

namespace TeamDataSource
{
    public class TeamFoundationServerWorkItemTeam : ITeamData
    {
        private readonly ITfsData m_data;
        private readonly Uri m_teamFoundationServerUri;
        private readonly string m_projectName;

        public TeamFoundationServerWorkItemTeam(ITfsData data, Uri teamFoundationServerUri, string projectName)
        {
            m_data = data;
            m_teamFoundationServerUri = teamFoundationServerUri;
            m_projectName = projectName;
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
            return InferParticipantsFromWorkItems(wi);
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

        public IEnumerable<Participant> InferParticipantsFromWorkItems(IEnumerable<WorkItem> workItems)
        {
            return InferParticipantsFromWorkItems(workItems, null);
        }

        public IEnumerable<Participant> InferParticipantsFromWorkItems(IEnumerable<WorkItem> workItems,
            Func<WorkItem, bool> predicate)
        {
            return InferParticipantsFromWorkItems(workItems, predicate, null);
        }

        public IEnumerable<Participant> InferParticipantsFromWorkItems(IEnumerable<WorkItem> workItems,
            Func<WorkItem, bool> workItemPredicate, Func<Revision, bool> workItemRevisionPredicate)
        {
            List<Participant> participants = new List<Participant>();

            if (workItemPredicate != null)
            {
                workItems = workItems.Where(workItemPredicate);
            }

            foreach (WorkItem workItem in workItems)
            {
                foreach (Revision revision in workItem.Revisions)
                {
                    if (workItemRevisionPredicate != null)
                    {
                        if (!workItemRevisionPredicate(revision))
                        {
                            continue;
                        }
                    }

                    participants.Add(new Participant((string)revision.Fields["Changed by"].Value));
                }
            }

            return participants.Distinct();
        }
    }


}
