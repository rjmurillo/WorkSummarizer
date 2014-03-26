using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using DataSources.Who.WhoServiceReference;

namespace DataSources.Who
{
    public class TeamProvider 
    {
        public string PullCommonManager(string alias1, string alias2)
        {
            throw new NotImplementedException();
        }

        public string PullManager(string alias)
        {
            var wsr = EnsurePeopleStoreSoapClient();
            var ctx = wsr.FindPersonContextByAlias(alias);
            var mgr = ctx.Managers.LastOrDefault();
            if (mgr != null)
            {
                return mgr.Alias;
            }
            return string.Empty;
        }

        public IEnumerable<string> PullDirectReports(string alias)
        {
            var wsr = EnsurePeopleStoreSoapClient();
            var ctx = wsr.FindPersonContextByAlias(alias);
            return ctx.DirectsChains.Select(s => s.Manager.Alias).Where(x => !x.Equals(String.Empty, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<string> PullPeers(string alias)
        {
            var directManager = PullManager(alias);
            var skipLevel = PullManager(directManager);

            var peers = new List<string>();

            foreach (var manager in PullDirectReports(skipLevel))
            {
                var reports = PullDirectReports(manager);
                if (reports != null)
                {
                    peers.AddRange(reports);
                }
            }

            return peers;
        }

        public string ResolveDisplayName(string displayName)
        {
            var wsr = EnsurePeopleStoreSoapClient();
            displayName = displayName.Trim(new[] {'\'', '\"', ' '});
            IEnumerable<Person> f = wsr.FindPeopleByFuzzy(displayName);
            f = f.OrderBy(q => q.Office, new BuildingPriorityComparer("27")); // prioritize office buildings...
            var p = f.FirstOrDefault(q => q.Name.Equals(displayName, StringComparison.OrdinalIgnoreCase)) 
                ?? (f.FirstOrDefault(q => q.Name.ToUpperInvariant().Contains(displayName.ToUpperInvariant())) 
                ?? f.FirstOrDefault());
            if (p != null)
            {
                return p.Alias;
            }

            return displayName;
        }

        private static PeopleStoreSoapClient EnsurePeopleStoreSoapClient()
        {
            var b = new BasicHttpBinding(BasicHttpSecurityMode.TransportCredentialOnly );
            b.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
            b.Security.Transport.ClientCredentialType = HttpClientCredentialType.Ntlm;
            b.MaxReceivedMessageSize = int.MaxValue;
            b.MaxBufferSize = int.MaxValue;
            b.MaxBufferPoolSize = int.MaxValue;
            b.ReaderQuotas.MaxDepth = 32;
            b.ReaderQuotas.MaxArrayLength = short.MaxValue;
            b.ReaderQuotas.MaxStringContentLength = int.MaxValue;

            var wsr = new PeopleStoreSoapClient(b, new EndpointAddress("http://who/PeopleStore.asmx"));
            
            
            return wsr;
        }

        private class BuildingPriorityComparer : IComparer<string>
        {
            private readonly string _priorityString;

            public BuildingPriorityComparer(string priorityString)
            {
                _priorityString = priorityString;
            }

            public int Compare(string x, string y)
            {
                var xPriority = x.StartsWith(_priorityString, StringComparison.OrdinalIgnoreCase);
                var yPriority = y.StartsWith(_priorityString, StringComparison.OrdinalIgnoreCase);

                if (xPriority && yPriority || !(xPriority || yPriority))
                {
                    return System.String.Compare(x, y, System.StringComparison.OrdinalIgnoreCase);
                }

                if (xPriority)
                {
                    return -1;
                }

                return 1;
            }
        }
    }
}
