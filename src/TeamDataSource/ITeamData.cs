using System.Collections.Generic;
using Common;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace WorkSummarizer.TeamDataSource
{
    public interface ITeamData
    {
        /// <summary>
        /// Gets a collection of participants that also touched files as part of the specified changesets.
        /// </summary>
        /// <param name="changesets"></param>
        /// <returns>A collection of <see cref="Participant"/> objects that also made changes to the
        /// specified files.</returns>
        /// <remarks>
        /// <code>
        /// var rimuri = TfsDataSource.PullWorkItemsThatChanged("2014-01-01", "2014-02-11");
        /// var otherParticipants = TeamDataSource.InferParticipantsFromChangesets(rimuri);
        /// </code>
        /// </remarks>
        IEnumerable<Participant> InferParticipantsFromChangesets(IEnumerable<Changeset> changesets);

        IEnumerable<Participant> InferParticipantsFromWorkItems(IEnumerable<WorkItem> workItems);
    }
}
