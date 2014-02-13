using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Events;
using TagCloud;

namespace Renders.HTML
{
    public class HtmlWriteEvents : IRenderEvents
    {
        public void Render(string eventType, IEnumerable<Event> events, IDictionary<string, int> weightedTags)
        {
            var sb = new StringBuilder(events.Count() * 250);

            sb.Append("<html><head><title>" + eventType + "</title><style type=\"text/css\">");
            BuildTagCloudCss(sb);
            sb.Append("</style></head><body>");

            if (weightedTags != null)
            {
                BuildTagCloud(sb, weightedTags);
            }

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

        private static void BuildTagCloud(StringBuilder sb, IDictionary<string, int> weightedTags)
        {
            var htmlTags =
                new TagCloud.TagCloud(
                    weightedTags,
                    new TagCloudGenerationRules {
                        MaxNumberOfTags = 100,
                        Order = TagCloudOrder.WeightDescending,
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
    }
}
