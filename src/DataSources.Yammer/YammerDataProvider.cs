using System;
using System.Collections.Generic;
using System.Linq;
using Yammer;
using Yammer.Schema;

namespace DataSources.Yammer
{
    public class YammerDataProvider
    {
        public static IEnumerable<YammerMessage> PullSentMessages(DateTime startFilterDate, DateTime endFilterDate)
        {
            YammerSession session = YammerAPI.LoginUserAsync("1GpcB6wgI9wqp8hB17PA", "R33cc1KWZ3uy6juE5DeNHWQKMjqzU3e7nvYkPLREeM", "http://qe");
            //var session = new YammerSession("");
            Console.WriteLine("Session: " + session.UserToken);

            var messages =
                PullSentMessages(session)
                    //.Where(p => p.CreatedAt.CompareTo(startFilterDate) > 1 && p.CreatedAt.CompareTo(endFilterDate) < 1)
                    .ToList();

            foreach (var message in messages)
            {
                Console.WriteLine("Sender: {0}", message.Sender);
                Console.WriteLine("\t{0}", message.Body.Substring(0, Math.Min(message.Body.Length, 70)).Replace("\n", ""));
                Console.WriteLine();
            }

            return messages;
        }

        public static IEnumerable<YammerMessage> PullSentMessages(YammerSession session)
        {
            bool hasNext = true;
            int lastMessageId = 0;

            while (hasNext)
            {
                var result = session.Messages.GetSentAsync(OlderThan: lastMessageId, Threaded: YammerMessages.Threaded.Extended).Result;
                var references = result.references.ToLookup(p => p.id);

                hasNext = result.messages.Any();

                foreach (var message in result.messages)
                {
                    var sender = references[message.sender_id].First().name;
                    lastMessageId = (int)message.id;
                    var createdAtString = message.GetPathValue("created_at", String.Empty);
                    var createdAt = DateTime.Parse(createdAtString);
                    yield return new YammerMessage(message.body.plain, sender, createdAt);
                }
            }
        }
    }
}
