
using System;
using System.Collections.Generic;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkSummarizer.TeamFoundationServerDataSource;

namespace WorkSummarizer.ZZTest.TeamFoundationServerDataSourceTests
{
    [TestClass]
    public class PullChangeSetTests
    {
        readonly Uri tfsConnection = new Uri("http://vstfcodebox:8080/tfs/Engineering_Excellence");
        private ITfsData tfsData;
        private readonly DateTime startDate = new DateTime(2014, 1, 1);
        private readonly DateTime endDate = new DateTime(2014, 2, 15);

        [TestInitialize]
        public void TestInitialize()
        {
            tfsData = new TfsData();
        }

        [TestMethod]
        public void ExceptionThrownIfBadUriForGettingChangeSets()
        {
            try
            {
                tfsData.PullChangeSets(new Uri("http://this:8080/will/not/work"), startDate, endDate);
            }
            catch (TeamFoundationException)
            {
                return;
            }
            Assert.Fail("Should have thrown an exception...");
        }

        [TestMethod]
        public void VerifyPullChangeSets()
        {
            IEnumerable<Changeset> changesets = tfsData.PullChangeSets(tfsConnection, startDate, endDate);

            foreach (Changeset cs in changesets)
            {
                Assert.IsTrue(!string.IsNullOrEmpty(cs.Comment));
            }
        }
    }
}
