using System;
using System.Collections.Generic;
using System.Linq;
using DataSources.Kudos.KudosDomainService;

namespace DataSources.Kudos
{
    public class KudosDataProvider : IDataPull<KudosPost>
    {
        public IEnumerable<KudosPost> PullData(DateTime startUtcTime, DateTime endUtcTime)
        {
            using (var kudosSoapClient = new KudosDomainServicesoapClient())
            {
                var alias = Environment.UserName.Split('\\').Last(); // REVIEW cheap inaccurate way to get alias
                Console.WriteLine("Pulling Kudos for " + alias);
                return kudosSoapClient.GetKudosReceived(null, alias)
                                      .Where(p => p.Date >= startUtcTime && p.Date <= endUtcTime)
                                      .Select(p => { return new KudosPost { Message = p.Message, SenderAlias = p.Alias, CreatedUtcTime = p.Date.ToUniversalTime() }; })
                                      .ToList();
            }
        }
    }
}
