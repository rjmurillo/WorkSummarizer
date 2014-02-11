

using System;
using System.Collections.Generic;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace WorkSummarizer.TeamFoundationServerDataSource
{
    public partial class TfsData : ITfsData
    {
        public IEnumerable<Changeset> PullChangeSets(Uri tfsConnectionstring, DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }
    }
}
