using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataSources.CodeFlow.CodeFlowDashboardService;

namespace DataSources.CodeFlow
{
    public class CodeFlowDataProvider
    {
        public static IEnumerable<CodeFlowReview> PullReviewsAuthored(DateTime startUtcTime, DateTime endUtcTime)
        {
            using (var codeFlowReviewClient = new ReviewDashboardServiceClient())
            {
                var alias = Environment.UserName.Split('\\').Last(); // REVIEW cheap inaccurate way to get user alias

                Console.WriteLine("Pulling reviews authored by " + alias);

                var result = codeFlowReviewClient.QueryReviewSummaries(new CodeReviewQuery
                {
                    CreatedBeforeDate = endUtcTime,
                    CreatedAfterDate = startUtcTime,
                    Authors = new[] { alias }, 
                    UserAgent = "EECTES/WorkSummarizer 1.0"
                });

                if (result.Reviews == null)
                {
                    return Enumerable.Empty<CodeFlowReview>();
                }

                return result.Reviews
                             .Select(p => 
                             {
                                 return new CodeFlowReview 
                                 {
                                     AuthorLogin = p.AuthorLogin,
                                     PublishedUtcDate = p.CreatedOn.ToUniversalTime(),
                                     ClosedUtcDate = p.CompletedOn.ToUniversalTime(),
                                     Name = p.Name
                                 };
                             })
                             .ToList();
            }
        }
    }
}
