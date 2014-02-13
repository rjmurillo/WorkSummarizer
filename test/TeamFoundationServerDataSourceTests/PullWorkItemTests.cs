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
        readonly Uri tfsConnection = new Uri("http://vstfcodebox:8080/tfs/Engineering_Excellence");
        private const string projectName = "EE_Engineering";

        private ITfsData tfsData;
        private readonly DateTime startDate = new DateTime(2014, 1, 1);
        private readonly DateTime endDate = new DateTime(2014, 2, 15);

        [TestInitialize]
        public void TestInitialize()
        {
            tfsData = new TeamFoundationServerDataProvider();
        }

        [TestMethod]
        public void ExceptionThrownIfBadUri()
        {
            try
            {
                tfsData.PullWorkItemsThatChanged(new Uri("http://this:8080/will/not/work"), projectName, startDate, endDate);
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
            IEnumerable<WorkItem> workItems =  tfsData.PullWorkItemsThatChanged(tfsConnection, projectName, startDate, endDate);

            foreach (WorkItem wi in workItems)
            {
                Assert.IsTrue(!string.IsNullOrEmpty(wi.Title));
            }
        }

        [TestMethod]
        public void VerifyPullWorkItemsOnlyForProject()
        {
            IEnumerable<WorkItem> workItems = tfsData.PullWorkItemsThatChanged(tfsConnection, projectName, startDate, endDate);

            foreach (WorkItem wi in workItems)
            {
                Assert.AreEqual(projectName, wi.Project.Name, "returned a project that should not have been returned");
            }
        }

        [TestMethod]
        public void VerifyPullWorkItemsDoesNotReturnTasksOrBugs()
        {
            IEnumerable<WorkItem> workItems = tfsData.PullWorkItemsThatChanged(tfsConnection, projectName, startDate, endDate);

            foreach (WorkItem wi in workItems)
            {
                Assert.AreNotEqual("Bug", wi.Type.Name, "should not have returned a bug");
            } 
        }
    }
}
