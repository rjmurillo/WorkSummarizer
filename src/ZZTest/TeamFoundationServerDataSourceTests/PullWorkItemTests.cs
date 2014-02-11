using System;
using System.Collections.Generic;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkSummarizer.TeamFoundationServerDataSource;

namespace WorkSummarizer.ZZTest.TeamFoundationServerDataSourceTests
{
    [TestClass]
    public class PullWorkItemTests
    {
        readonly Uri tfsConnection = new Uri("http://vstfcodebox:8080/tfs/");
        private ITfsData tfsData;
        private readonly DateTime startDate = new DateTime(2014, 1, 1);
        private readonly DateTime endDate = new DateTime(2014, 2, 15);

        [TestInitialize]
        public void TestInitialize()
        {
            tfsData = new TfsData();
        }

        [TestMethod]
        public void ExceptionThrownIfBadUri()
        {
            try
            {
                tfsData.PullWorkItemsThatChanged(new Uri("http://this:8080/will/not/work"), startDate, endDate);
            }
            catch (TeamFoundationException)
            {
                return;
            }
            Assert.Fail("Should have thrown an exception...");
        }

        [TestMethod]
        public void VerifyPullWorkItems()
        {
            IEnumerable<WorkItem> workItems =  tfsData.PullWorkItemsThatChanged(tfsConnection, startDate, endDate);

            foreach (WorkItem wi in workItems)
            {
                Assert.IsTrue(!string.IsNullOrEmpty(wi.Title));
            }
        }

        [TestMethod]
        public void VerifyPullWorkItemsHandlesStartDateGreaterThanEndDate()
        {
             IEnumerable<WorkItem> workItems = tfsData.PullWorkItemsThatChanged(tfsConnection, new DateTime(2014, 1, 1), new DateTime(2013, 2, 15));
        }
    }
}
