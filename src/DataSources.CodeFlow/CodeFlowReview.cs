using System;
using System.Collections;
using System.Collections.Generic;
using Common;

namespace DataSources.CodeFlow
{
    public class CodeFlowReview
    {
        public string AuthorLogin { get; set; }
        public string Name { get; set; }
        public DateTime PublishedUtcDate { get; set; }
        public DateTime ClosedUtcDate { get; set; }

        public IEnumerable<Participant> Reviewers { get; set; }
    }
}
