using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using DataSources.Who;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace DataSources.TeamFoundationServer
{


    public class TeamFoundationServerWorkItemTeamDataProvider : IInferTeam<WorkItem>
    {
        public IEnumerable<Participant> InferParticipants(IEnumerable<WorkItem> workItems)
        {
            return InferParticipants(workItems, null);
        }

        public IEnumerable<Participant> InferParticipants(IEnumerable<WorkItem> workItems,
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

                    participants.Add(IdentityUtility.Create((string)revision.Fields["Changed by"].Value));
                }
            }

            return participants.Distinct();
        }
    }


}
