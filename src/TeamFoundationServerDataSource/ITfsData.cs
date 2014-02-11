

using System;
using System.Collections.Generic;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace WorkSummarizer.TeamFoundationServerDataSource
{
    public interface ITfsData
    {
        /// <summary>
        /// Gets a collection of workitems that changed within the time period
        /// </summary>
        /// <param name="tfsConnectionstring">the TFS TPC store to query</param>
        /// <param name="startDate">start of the query time period</param>
        /// <param name="endDate">end of the query time period</param>
        /// <returns>collection of tfs workitems</returns>
        IEnumerable<WorkItem> PullWorkItemsThatChanged(Uri tfsConnectionstring, DateTime startDate, DateTime endDate);

        /// <summary>
        /// Gets a collection of changesets that happened within the time period
        /// </summary>
        /// <param name="tfsConnectionstring">the TFS TPC store to query</param>
        /// <param name="startDate">start of the query time period</param>
        /// <param name="endDate">end of the query time period</param>
        /// <returns>collection of tfs changesets</returns>
        IEnumerable<Changeset> PullChangeSets(Uri tfsConnectionstring, DateTime startDate, DateTime endDate);
    }
}
