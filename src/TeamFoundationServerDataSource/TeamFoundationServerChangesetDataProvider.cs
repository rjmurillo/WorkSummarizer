using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.Win32;
using WorkSummarizer.TeamFoundationServerDataSource;

namespace DataSources.TeamFoundationServer
{
    public class TeamFoundationServerChangesetDataProvider : IDataPull<Changeset>
    {
        public TeamFoundationServerChangesetDataProvider(string visualStudioVersion = "12.0")
        {
            VisualStudioVersion = visualStudioVersion;
        }

        public string VisualStudioVersion { get; private set; }

        public IEnumerable<Changeset> PullData(DateTime startDateTime, DateTime endDateTime)
        {
            List<Changeset> retval = new List<Changeset>();

            foreach (var connection in CollectionUri())
            {
                var t = new TeamFoundationServerChangesetDataProviderInternal(connection, null);
                retval.AddRange(t.PullData(startDateTime, endDateTime));
            }

            return retval;
        }

        private IEnumerable<Uri> CollectionUri()
        {
            List<Uri> retval = new List<Uri>();

            //HKCU\Software\Microsoft\VisualStudio\nn.0\TeamFoundation\Instances\<INSTANCE NAME>\Collections\<COLLECTION NAME>

            RegistryKey registryKey =
                Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\VisualStudio\\" + VisualStudioVersion + "\\TeamFoundation\\Instances");
            try
            {
                foreach (var instanceName in registryKey.GetSubKeyNames())
                {
                    RegistryKey instance = registryKey.OpenSubKey(instanceName, RegistryKeyPermissionCheck.ReadSubTree);
                    try
                    {
                        var collections = instance.GetSubKeyNames();
                        foreach (var collectionContainer in collections)
                        {
                            RegistryKey collectionContainerRegistryKey = instance.OpenSubKey(
                                collectionContainer,
                                RegistryKeyPermissionCheck.ReadSubTree);
                            try
                            {
                                foreach (var collection in collectionContainerRegistryKey.GetSubKeyNames())
                                {
                                    RegistryKey collectionRegistryKey =
                                        collectionContainerRegistryKey.OpenSubKey(collection);

                                    try
                                    {
                                        string uri = collectionRegistryKey.GetValue("Uri") as string;
                                        retval.Add(new Uri(uri));
                                    }
                                    finally
                                    {
                                        collectionRegistryKey.Close();
                                        collectionRegistryKey.Dispose();
                                    }
                                }
                            }
                            finally
                            {
                                collectionContainerRegistryKey.Close();
                                collectionContainerRegistryKey.Dispose();
                            }
                        }
                    }
                    finally
                    {
                        instance.Close();
                        instance.Dispose();
                    }
                }
            }
            finally
            {
                registryKey.Close();
                registryKey.Dispose();
            }

            return retval;
        }


    }

    internal class TeamFoundationServerChangesetDataProviderInternal : IDataPull<Changeset>
    {
        public TeamFoundationServerChangesetDataProviderInternal(Uri tfsConnectionString, string projectName)
        {
            TeamFoundationServer = tfsConnectionString;
            Project = projectName;
        }

        public Uri TeamFoundationServer { get; private set; }
        public string Project { get; private set; }

        public IEnumerable<Changeset> PullData(DateTime startDate, DateTime endDate)
        {
            try
            {
                VersionSpec versionFrom = new DateVersionSpec(startDate);
                VersionSpec versionTo = new DateVersionSpec(endDate);

                TfsTeamProjectCollection projectCollection =
                    TfsTeamProjectCollectionFactory.GetTeamProjectCollection(TeamFoundationServer);
                VersionControlServer versionControlServer =
                    (VersionControlServer)projectCollection.GetService(typeof(VersionControlServer));

                string scope = null;
                if (string.IsNullOrWhiteSpace(Project))
                {
                    scope = "$/*";
                }
                else
                {
                    scope = "$/" + Project + "/";
                }

                IEnumerable changesetHistory =
                    versionControlServer.QueryHistory(
                        scope,
                        VersionSpec.Latest,
                        0,
                        RecursionType.Full,
                        null,
                        versionFrom,
                        versionTo,
                        int.MaxValue,
                        false,
                        false);

                return changesetHistory.Cast<Changeset>().ToList();
            }
            catch (Exception ex)
            {
                throw new TeamFoundationException(
                    "Unable to get versionControlServer for TFS server " + TeamFoundationServer.AbsoluteUri, ex);
            }
        }

        public IEnumerable<Changeset> PullChangesets(Uri tfsConnectionString, IEnumerable<int> changesetIds)
        {
            TfsTeamProjectCollection projectCollection =
                TfsTeamProjectCollectionFactory.GetTeamProjectCollection(tfsConnectionString);
            VersionControlServer versionControlServer =
                (VersionControlServer)projectCollection.GetService(typeof(VersionControlServer));

            return changesetIds.Select(versionControlServer.GetChangeset);
        }
    }
}