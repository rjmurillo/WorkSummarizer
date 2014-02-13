﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Extensibility;

namespace Events
{
    public class FakeEventsPlugin
    {
        public FakeEventsPlugin(IPluginRuntime pluginRuntime)
        {
            pluginRuntime.EventQueryServices["Fake"] = new FakeEventsQueryService();
        }

        private class FakeEventsQueryService : IEventQueryService
        {
            private readonly Random m_random = new Random((int)DateTime.Now.Ticks);

            public IEnumerable<Event> PullEvents(DateTime startDateTime, DateTime stopDateTime)
            {
                return Enumerable.Range(0, 100).Select(p => new Event { Date = startDateTime, EventType = "Fake", Subject = new Subject { Text = "Fake subject " + p }, Text = m_random.Next(100) + " is the meaning of life, the universe and everything" });
            }


            public IEnumerable<Event> PullEvents(DateTime startDateTime, DateTime stopDateTime, Func<Event, bool> predicate)
            {
                return Enumerable.Range(0, 100).Select(p => new Event { Date = startDateTime, EventType = "Fake", Subject = new Subject { Text = "Fake subject " + p }, Text = m_random.Next(100) + " is the meaning of life, the universe and everything" });
            }
        }
    }
}
