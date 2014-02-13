using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace DataSources.Team
{
    public class TeamFoundationServerWorkItemTeamDataProvider 
    {
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
