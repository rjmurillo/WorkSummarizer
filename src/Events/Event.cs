using System;
using System.Collections;
using Common;
using Graph;

namespace Events
{
    public class Event
    {
        public Event()
        {
            Text = String.Empty;
            Participants = new Graph<Participant>();
            Subject = new Subject {Text = String.Empty};
            EventType = String.Empty;
        }

        public string Text { get; set; }
        public DateTime Date { get; set; }
        public double Duration { get; set; }

        public Graph<Participant> Participants { get; set; }

        public Subject Subject { get; set; }

        public string EventType { get; set; }

        public object Context { get; set; }

    }
}