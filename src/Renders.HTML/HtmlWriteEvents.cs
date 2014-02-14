using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Events;
using TagCloud;

namespace Renders.HTML
{
    public static class TimeSpanExtensions
    {
        public static int GetYears(this TimeSpan timespan)
        {
            return (int)((double)timespan.TotalDays / 365.2425);
        }
        public static int GetMonths(this TimeSpan timespan)
        {
            return (int)((double)timespan.TotalDays / 30.436875);
        }

        public static int GetWeeks(this TimeSpan timespan)
        {
            return (int)((double)timespan.TotalDays / 7);
        }
    }

    public class HtmlWriteEvents : IRenderEvents
    {
        private static string LoadResource(string name)
        {
            var a = Assembly.GetExecutingAssembly();
            using (var s = a.GetManifestResourceStream("Renders.HTML." + name))
            {
                Debug.Assert(s != null);
                using (var r = new StreamReader(s))
                {
                    return r.ReadToEnd();
                }
            }
        }

        public void Render(string eventType, DateTime startDateTime, DateTime endDateTime, IEnumerable<Event> events, IDictionary<string, int> weightedTags, IDictionary<string, int> weightedPeople, IEnumerable<string> importantSentences)
        {
            var sb = new StringBuilder(events.Count() * 250);

            if (!eventType.Equals("Summary"))
            {
                sb.AppendLine("<h1>Work Summary - " + HtmlEscape(eventType) + "</h1>");
            }
            else
            {
                sb.AppendLine("<h1>Work Summary - " + startDateTime.ToShortDateString() + " to " + endDateTime.ToShortDateString() + "</h1>");

                var ts = endDateTime - startDateTime;

                var g = from e in events
                    group e by e.EventType
                    into grp
                    select new
                    {
                        key = grp.Key, 
                        count = grp.Count(),
                        avgDays = grp.Count() / ts.TotalDays,
                        avgHours = grp.Count() / (ts.TotalHours * 0.33d),
                        hours = grp.Sum(s=>s.Duration.Hours)
                    };

                DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
                Calendar cal = dfi.Calendar;

                var g1 = from e in events
                    group e by new
                    {
                        type = e.EventType,
                        month = e.Date.Month
                    }
                    into grp
                    select new
                    {
                        type = grp.Key.type,
                        month = grp.Key.month,
                        monthCount = grp.Count()
                    };

                var g2 = from e in events
                    group e by new
                    {
                        type = e.EventType,
                        week = cal.GetWeekOfYear(e.Date, dfi.CalendarWeekRule, dfi.FirstDayOfWeek)
                    }
                    into grp
                    select new
                    {
                        type = grp.Key.type,
                        week = grp.Key.week,
                        weekCount = grp.Count()
                    };

                var g3 = from e in events
                    group e by new
                    {
                        type = e.EventType,
                        day = e.Date.Date
                    }
                    into grp
                    select new
                    {
                        type = grp.Key.type,
                        day = grp.Key.day,
                        dayCount = grp.Count()
                    };

                
                
                
                //sb.AppendLine("<table id=\"overall_summary\" data-role=\"table\"><thead><tr><th>Event Type</th><th>Count</th><th>Total Hours<th><th>Avg / Day</th><th>Avg / Hour</th></tr></thead><tbody>");
                //foreach (var r in g.OrderByDescending(ks=>ks.count))
                //{
                //    sb.AppendLine("<tr><td>" + r.key + "</td><td>" + r.count + "</td><td>" + r.hours + "</td><td>" + r.avgDays + "</td><td>" + r.avgHours + "</td></tr>");
                //}
                //sb.AppendLine("</tbody></table>");

                foreach (var r in g)
                {
                    sb.AppendLine("<div class=\"container\">");

                    sb.AppendLine("<div class=\"meetings-attended\">");
                    sb.AppendLine("<span>" + r.count + "</span>");
                    sb.AppendLine("<h2>" + DetermineHeader(r.key) + "</h2>");
                    sb.AppendLine("</div>");

                    sb.AppendLine("<div class=\"hours\">");
                    sb.AppendLine("<span>" + Math.Round(r.avgDays, 1) + "</span>");
                    sb.AppendLine("<p>" + DetermineHeader(r.key) + "<br /> per day (on average)</p>");
                    sb.AppendLine("</div>");

                    sb.AppendLine("<div class=\"hours\">");
                    sb.AppendLine("<span>" + Math.Round(r.avgHours, 1) + "</span>");
                    sb.AppendLine("<p>" + DetermineHeader(r.key) + "<br /> per hour (on average)</p>");
                    sb.AppendLine("</div>");

                    sb.Append("</div>");
                }
                


            }

            if (weightedPeople != null)
            {
                sb.AppendLine("<h2>Connections</h2>");
                BuildTagCloud(sb, weightedPeople);
            }

            if (weightedTags != null)
            {
                sb.AppendLine("<h2>Frequent words</h2>");
                BuildTagCloud(sb, weightedTags);
            }

            if (importantSentences != null)
            {
                sb.AppendLine("<h2>Important events</h2>");
                sb.AppendLine("<ul>");
                foreach (var sentence in importantSentences)
                {
                    sb.AppendLine("<li>" + HtmlEscape(sentence) + "</li>");
                }
                sb.AppendLine("</ul>");
            }
            if (events != null)
            {
                sb.AppendLine("<h2>Events</h2>");
                foreach (var evnt in events)
                {
                    BuildEventHtml(sb, evnt);
                }
            }
            
            /* Boilerplate has 5 areas
             * 0: Title
             * 1: CSS
             * 2: JS (blocking JS)
             * 3: Your Content
             * 4: Your other JS
             */
            var htmlBoilerplate = LoadResource("boilerplate2.html");
            var cssNormalize = LoadResource("normalize.css");
            var cssMain = LoadResource("main.css");
            var cssTagCloud = BuildTagCloudCss();
            var jsMain = LoadResource("main.js");
            var jsOther = BuildOtherJs(weightedTags);

            var htmlFinal = string.Format(
                CultureInfo.InvariantCulture,
                htmlBoilerplate,
                HtmlEscape(eventType),
                cssNormalize + cssMain + cssTagCloud,
                string.Empty,
                sb,
                jsMain + jsOther);

            string fileName = string.Format("WorkSummarizer_{0}.html", eventType);
            File.WriteAllText(fileName, htmlFinal);
            Process.Start(fileName);

        }

        private string BuildOtherJs(IDictionary<string, int> weightedTags)
        {
            var sb = new StringBuilder();

            sb.AppendLine("$('#important_events').wrapInTag({");
sb.AppendLine("    tag: 'strong',");
sb.Append("    words: [");
            foreach (var t in weightedTags.Keys)
            {
                sb.Append("'" + t + "',");
            }
            sb.Append("'blargelfarf'");
            sb.AppendLine("]");
sb.AppendLine("});");

            return sb.ToString();
        }

        private string DetermineHeader(string p)
        {
            switch (p)
            {
                case "CodeFlow.Author":
                    return "Code Reviews";
                case "Connect.Submission":
                    return "Connect Reviews";
                case "Kudos.Received":
                    return "Kudos Recieved";
                case "ManicTime Activities":
                    return "ManicTime Activities";
                case "Outlook.ConversationHistory":
                    return "Lync Chats";
                case "Outlook.Email":
                    return "Email Sent";
                case "Outlook.Meeting":
                    return "Meetings Attended";
                case "TeamFoundationServer.Changeset":
                    return "Changesets Comitted";
                case "TeamFoundationServer.WorkItem.Resolved":
                    return "Work Items Resolved";
                case "TeamFoundationServer.WorkItem.Revision":
                    return "Work Items Edited";
                case "TeamFoundationServer.WorkItem.Closed":
                    return "Work Items Closed";
                case "TeamFoundationServer.WorkItem.Activated":
                    return "Work Items Activated";
                case "TeamFoundationServer.WorkItem.Created":
                    return "Work Items Created";
                case "Yammer.SentMessages":
                    return "Yams Sent";
                case "ManicTime.Activity":
                    return "ManicTime Activities";
                default:
                    return p;
            }
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

        private string BuildTagCloudCss()
        {
            return @"
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
                }";
        }

        private static string HtmlEscape(string text)
        {
            return text.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
        }
    }
}
