using System;
using System.Collections.Generic;
using Yammer;

namespace Events.Yammer
{
    public class YammerEventsQueryService
    {
        public static IEnumerable<YammerEvent> PullUserMessages(DateTime startFilterDate, DateTime endFilterDate)
        {
            YammerSession session = YammerAPI.LoginUserAsync("1GpcB6wgI9wqp8hB17PA", "R33cc1KWZ3uy6juE5DeNHWQKMjqzU3e7nvYkPLREeM", "https://www.yammer.com");
            //var session = new YammerSession("e2c39b1509a85c85319ec8bdfaa247b0");
            var currentUser = session.Users.GetCurrentUserAsync().Result;

            return null;
        }
    }
}