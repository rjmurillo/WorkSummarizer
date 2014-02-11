

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
        /// <param name="tfsConnectionstring">the TFS TPC store to query: http://vstfcodebox:8080/tfs/Engineering_Excellence</param>
        /// <param name="projectName">Name of the project in the TPC to scope to: EE_Engineering</param>
        /// /// <param name="startDate">start of the query time period</param>
        /// <param name="endDate">end of the query time period</param>
        /// <returns>collection of tfs workitems</returns>
        /// <!--<exception cref="TeamFoundationException">Thrown if an error occurs</exception>-->
        IEnumerable<WorkItem> PullWorkItemsThatChanged(Uri tfsConnectionstring, string projectName, DateTime startDate, DateTime endDate);

        /// <summary>
        /// Gets a collection of changesets that happened within the time period
        /// </summary>
        /// <param name="tfsConnectionstring">the TFS TPC store to query: http://vstfcodebox:8080/tfs/Engineering_Excellence</param>
        /// <param name="projectName">Name of the project in the TPC to scope to: EE_Engineering</param>
        /// <param name="startDate">start of the query time period</param>
        /// <param name="endDate">end of the query time period</param>
        /// <returns>collection of tfs changesets</returns>
        /// <!--<exception cref="TeamFoundationException">Thrown if an error occurs</exception>-->
        IEnumerable<Changeset> PullChangeSets(Uri tfsConnectionstring, string projectName, DateTime startDate, DateTime endDate);
    }
}
