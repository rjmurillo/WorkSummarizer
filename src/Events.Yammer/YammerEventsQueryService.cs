using System;
using System.Collections.Generic;
using System.Linq;
using FUSE.Weld.Base;
using Yammer;

namespace Events.Yammer
{
    public class YammerEventsQueryService
    {
        public static IEnumerable<YammerEvent> PullUserMessages(DateTime startFilterDate, DateTime endFilterDate)
        {
            YammerSession session = YammerAPI.LoginUserAsync("1GpcB6wgI9wqp8hB17PA", "R33cc1KWZ3uy6juE5DeNHWQKMjqzU3e7nvYkPLREeM", "http://qe");
            //var session = new YammerSession("");
            Console.WriteLine("Session: " + session.UserToken);
            var currentUser = session.Users.GetCurrentUserAsync().Result;

            var thing = session.Messages.GetSentAsync(Threaded: YammerMessages.Threaded.Extended, Limit: 100).Result;
            var references = thing.references.ToLookup(p => p.id);
            foreach (var message in thing.messages)
            {

                Console.WriteLine("Sender: {0}", references[message.sender_id].First().name);
                Console.WriteLine("\t{0}", message.body.plain.Substring(0, Math.Min(message.body.plain.Length, 70)).Replace("\n", ""));
                Console.WriteLine();
            }
            
            return null;
        }
    }
}