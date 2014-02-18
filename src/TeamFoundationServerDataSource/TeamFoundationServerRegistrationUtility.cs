using System;
using System.Collections.Generic;
using Microsoft.Win32;

namespace DataSources.TeamFoundationServer
{
    public static class TeamFoundationServerRegistrationUtility
    {
        public static IEnumerable<Uri> LoadRegisteredTeamFoundationServers(string VisualStudioVersion = "12.0")
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
}