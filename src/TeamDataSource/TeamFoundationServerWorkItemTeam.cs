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
            List<Participant> participants = new List<Participant>();

            foreach (WorkItem workItem in workItems)
            {
                foreach (Revision revision in workItem.Revisions)
                {
                    participants.Add(new Participant() { Alias = (string)revision.Fields["Changed by"].Value });
                }
            }

            return participants.Distinct();
        }
    }


}
