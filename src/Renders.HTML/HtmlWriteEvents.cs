using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Events;
using TagCloud;

namespace Renders.HTML
{
    public class HtmlWriteEvents : IRenderEvents
    {
        public void Render(string eventType, IEnumerable<Event> events, IDictionary<string, int> weightedTags, IDictionary<string, int> weightedPeople, IEnumerable<string> importantSentences)
        {
            var sb = new StringBuilder(events.Count() * 250);

            sb.Append("<html><head><title>" + HtmlEscape(eventType) + "</title><style type=\"text/css\">");
            sb.Append("h1,h2 { font-family:arial;}");
            BuildTagCloudCss(sb);
            sb.Append("</style></head><body>");
            sb.Append("<h1>Work Summary</h1>");

            if (weightedTags != null)
            {
                sb.Append("<h2>Top words:</h2>");
                BuildTagCloud(sb, weightedTags);
            }

            if (importantSentences != null)
            {
                sb.Append("<h2>Important Sentences:</h2>");
                foreach (var sentence in importantSentences)
                {
                    sb.Append("<p>" + HtmlEscape(sentence) + "</p>");
                }
            }

            if (weightedPeople != null)
            {
                sb.Append("<h2>People:</h2>");
                BuildTagCloud(sb, weightedPeople);
            }

            if (events != null)
            {
                sb.Append("<h2>Events:</h2>");
                foreach (var evnt in events)
                {
                    BuildEventHtml(sb, evnt);
                }
            }
            sb.Append("</body></html>");

            string fileName = string.Format("WorkSummary_{0}.html", eventType);
            File.WriteAllText(fileName, sb.ToString());
            Process.Start(fileName);

        }

        private static void BuildEventHtml(StringBuilder sb, Event evnt)
        {
            sb.Append("<p>");
            sb.Append("<div style=\"font-weight:bold;\">type: " + HtmlEscape(evnt.EventType) + "</div>");

            sb.Append("<div>date: " + evnt.Date);
            if (evnt.Duration.Ticks > 0)
            {
                sb.Append(" - duration: " + evnt.Duration);
            }
            sb.Append("</div>");

            if (evnt.Subject != null && !string.IsNullOrWhiteSpace(evnt.Subject.Text))
            {
                sb.Append("<div>subject: " + HtmlEscape(evnt.Subject.Text) + "</div>");
            }
            if (evnt.Context != null)
            {
                sb.Append("<div>context: " + HtmlEscape(evnt.Context.ToString()) + "</div>");
            }
            if (evnt.Participants != null)
            {
                sb.Append("<div>participants: ");
                string separator = string.Empty;
                foreach (var participant in evnt.Participants)
                {
                    sb.Append(separator);
                    separator = ", ";
                    sb.Append(HtmlEscape(participant.Value.Alias));
                }
                sb.Append("</div>");
            }
            if (!string.IsNullOrWhiteSpace(evnt.Text))
            {
                sb.Append("<div>text: <div style=\"border:1px solid #9AF\">" + HtmlEscape(evnt.Text) + "</div></div>");
            }
            sb.Append("</p>");
        }

        private static void BuildTagCloud(StringBuilder sb, IDictionary<string, int> weightedTags)
        {
            var htmlTags =
                new TagCloud.TagCloud(
                    weightedTags,
                    new TagCloudGenerationRules {
                        MaxNumberOfTags = 100,
                        Order = TagCloudOrder.Centralized,
                        TagCssClassPrefix = "TagWeight",
                    }).ToString();
            sb.Append("<p>" + htmlTags + "</p>");
        }

        private static void BuildTagCloudCss(StringBuilder sb)
        {
            sb.Append(@"
                .TagCloud			/* Applies to the entire tag cloud */
                {
	                font-family:Trebuchet MS;
	                border:1px solid #888;
	                padding:3px; 
	                text-align:center;
                }

                .TagCloud > span	/* Applies to each tag of the tag cloud */
                {
	                margin-right:3px;
	                text-align:center;
                }

                .TagCloud > span.TagWeight1	/* Applies to the largest tags */
                {
	                font-size:40px;
                }

                .TagCloud > span.TagWeight2
                {
	                font-size:32px;
                }

                .TagCloud > span.TagWeight3
                {
	                font-size:25px;
                }

                .TagCloud > span.TagWeight4
                {
	                font-size:18px;
                }

                .TagCloud > span.TagWeight5	/* Applies to the smallest tags */
                {
	                font-size:12px;
                }");
        }

        private static string HtmlEscape(string text)
        {
            return text.Replace("<", "&lt;").Replace(">", "&gt;");
        }
    }
}
