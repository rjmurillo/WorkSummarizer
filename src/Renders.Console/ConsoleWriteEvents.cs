using System;
using System.Collections.Generic;
using Events;

namespace Renders.Console
{
    public class ConsoleWriteEvents : IRenderEvents
    {
        public void WriteOut(string eventType, IEnumerable<Event> events)
        {
            foreach (Event evt in events)
            {
                System.Console.WriteLine("{0} {1}: {2}...", evt.Date.ToLocalTime(), evt.Subject.Text, evt.Text.Substring(0, Math.Min(evt.Text.Length, 30)).Replace("\n", String.Empty));
            }
        }
    }
}
