using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

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
            var wsr = new WhoServiceReference.PeopleStoreSoapClient(new BasicHttpBinding() {}, new EndpointAddress("http://who/PeopleStore.asmx"));
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
            var wsr = new WhoServiceReference.PeopleStoreSoapClient(new BasicHttpBinding() { }, new EndpointAddress("http://who/PeopleStore.asmx"));
            var ctx = wsr.FindPersonContextByAlias(alias);

            return ctx.DirectsChains.Select(s => s.Manager.Alias);
        }
    }
}
