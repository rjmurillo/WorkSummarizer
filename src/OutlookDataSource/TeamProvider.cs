using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using DataSources.Who;

namespace DataSources.Outlook
{
    public class TeamProvider : IInferTeam<OutlookItem>
    {

        public IEnumerable<Participant> InferParticipants(IEnumerable<OutlookItem> data)
        {
            return InferParticipants(data, null);
        }

        public IEnumerable<Participant> InferParticipants(IEnumerable<OutlookItem> data, Func<OutlookItem, bool> predicate)
        {
            return data.SelectMany(s => s.Recipients.Select(IdentityUtility.Create));
        }
    }
}