﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Yammer;

namespace DataSources.Yammer
{
    public class YammerDataProvider : IDataPull<YammerMessage>
    {
        public IEnumerable<YammerMessage> PullData(DateTime startFilterDate, DateTime endFilterDate)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            var userTokenConfig = config.AppSettings.Settings["YammerSession.UserToken"];
            var userToken = userTokenConfig != null ? userTokenConfig.Value : String.Empty;
            var session = new YammerSession(userToken);

            bool isSessionValid = false;
            if (!String.IsNullOrWhiteSpace(userToken))
            {
                try
                {
                    // test the token...
                    session.Users.GetCurrentUserAsync().Wait();
                    isSessionValid = true;
                }
                catch (AggregateException)
                {
                    Console.WriteLine("Session token invalid");
                }
            }

            if(!isSessionValid)
            {
                session = YammerAPI.LoginUserAsync("1GpcB6wgI9wqp8hB17PA", "R33cc1KWZ3uy6juE5DeNHWQKMjqzU3e7nvYkPLREeM", "http://qe");
                config.AppSettings.Settings.Remove("YammerSession.UserToken");
                config.AppSettings.Settings.Add("YammerSession.UserToken", session.UserToken);
                config.Save(ConfigurationSaveMode.Full);
                ConfigurationManager.RefreshSection("appSettings");
            }

            Console.WriteLine("Session token: " + session.UserToken);

            return PullSentMessages(session, startFilterDate).Where(p => p.CreatedAt >= startFilterDate && p.CreatedAt <= endFilterDate).ToList();
        }

        private static IEnumerable<YammerMessage> PullSentMessages(YammerSession session, DateTime startDate)
        {
            bool hasNext = true;
            int lastMessageId = 0;

            while (hasNext)
            {
                var result = session.Messages.GetSentAsync(OlderThan: lastMessageId, Threaded: YammerMessages.Threaded.Extended, Limit: 100).Result;
                var references = result.references.ToLookup(p => p.id);

                hasNext = result.messages.Any();

                foreach (var message in result.messages)
                {
                    var sender = references[message.sender_id].First().name;
                    lastMessageId = (int)message.id;
                    var createdAtString = message.GetPathValue("created_at", String.Empty);
                    var createdAt = DateTime.Parse(createdAtString);

                    if (createdAt < startDate)
                    {
                        hasNext = false;
                    }

                    yield return new YammerMessage(message.body.plain, sender, createdAt);
                }
            }
        }
    }
}
