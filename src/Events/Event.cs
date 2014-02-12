using System;
using System.Collections;
using Common;
using TeamFoundationServerWorkItemStateUpdater.Graph;

namespace Events
{
    public class Event
    {
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public double Duration { get; set; }

        public Graph<Participant> Participants { get; set; }

        public Subject Subject { get; set; }

        public string EventType { get; set; }

        public object Context { get; set; }

    }
}