﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using DataSources.Kudos;
using DataSources.Who;

namespace Events.Kudos
{
    public class KudosReceivedEventQueryService : EventQueryServiceBase
    {
        public override IEnumerable<Event> PullEvents(DateTime startDateTime, DateTime stopDateTime, Func<Event, bool> predicate)
        {
            var kdp = new KudosDataProvider();

            var retval = kdp.PullData(startDateTime, stopDateTime)
                .Select(p => 
                {
                    var e = new Event
                    {
                        Date = p.CreatedUtcTime,
                        EventType = "Kudos.Received",
                        Context = p.SenderAlias ,
                        Text = p.Message
                    };

                    e.Participants.Add(IdentityUtility.Create(p.SenderAlias));
                    e.Participants.Add(IdentityUtility.Create(Environment.UserName));

                    return e;
                }).ToList();

            return (predicate != null) ? retval.Where(predicate) : retval;
        }
    }
}
