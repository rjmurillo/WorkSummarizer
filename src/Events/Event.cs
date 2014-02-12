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

        public Graph<Participant> Nodes { get; set; }

        public Subject Subject { get; set; }

    }
}