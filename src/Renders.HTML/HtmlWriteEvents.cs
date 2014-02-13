using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Events;

namespace Renders.HTML
{
    public class HtmlWriteEvents : IRenderEvents
    {
        public void WriteOut(string eventType, IEnumerable<Event> events)
        {
            var sb = new StringBuilder(events.Count() * 250);

            sb.Append("<html><head><title>" + eventType + "</title></head><body>");
            foreach (var evnt in events)
            {
                BuildHtmlFragment(sb, evnt);
            }
            sb.Append("</body></html>");

            string fileName = string.Format("WorkSummary_{0}.html", eventType);
            File.WriteAllText(fileName, sb.ToString());
            Process.Start(fileName);

        }

        private static void BuildHtmlFragment(StringBuilder sb, Event evnt)
        {
            sb.Append("<p>");
            sb.Append("<div style=\"font-weight:bold;\">type: " + evnt.EventType + "</div>");

            sb.Append("<div>date: " + evnt.Date);
            if (evnt.Duration.Ticks > 0)
            {
                sb.Append(" - duration: " + evnt.Duration);
            }
            sb.Append("</div>");

            if (evnt.Subject != null && !string.IsNullOrWhiteSpace(evnt.Subject.Text))
            {
                sb.Append("<div>subject: " + evnt.Subject.Text + "</div>");
            }
            if (evnt.Context != null)
            {
                sb.Append("<div>context: " + evnt.Context + "</div>");
            }
            if (evnt.Participants != null)
            {
                sb.Append("<div>participants: ");
                string separator = string.Empty;
                foreach (var participant in evnt.Participants)
                {
                    sb.Append(separator);
                    separator = ", ";
                    sb.Append(participant.Value.Alias);
                }
                sb.Append("</div>");
            }
            if (!string.IsNullOrWhiteSpace(evnt.Text))
            {
                sb.Append("<div>text: <div style=\"border:1px solid #9AF\">" + evnt.Text + "</div></div>");
            }
            sb.Append("</p>");
        }
    }
}
