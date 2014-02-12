using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using WorkSummarizer.TeamDataSource;

namespace TeamDataSource
{
    public class TeamFoundationServerWorkItemTeam : ITeamData
    {
        public IEnumerable<Participant> InferParticipantsFromChangesets(IEnumerable<Microsoft.TeamFoundation.VersionControl.Client.Changeset> changesets)
        {
            throw new NotImplementedException();
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

                    participants.Add(new Participant() { Alias = (string)revision.Fields["Changed by"].Value });
                }
            }

            return participants.Distinct();
        }
    }


}
