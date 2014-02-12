using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WorkSummarizer
{
    public static class TfsHelper
    {
        public static IDictionary<int, Changeset> CreateChangesetIdMap(IEnumerable<Changeset> changesets)
        {
            return changesets.ToDictionary(o => o.ChangesetId, o => o);
        }

        public static IDictionary<int, WorkItem> CreateWorkItemIdMap(IEnumerable<WorkItem> workitems)
        {
            return workitems.ToDictionary(o => o.Id);
        }
    }
}
