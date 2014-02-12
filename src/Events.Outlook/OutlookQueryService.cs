using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Outlook;

namespace Events.Outlook
{
    public class OutlookQueryService
    {
        public static IEnumerable<OutlookMeeting> GetMeetings(DateTime startFilterDate, DateTime endFilterDate)
        {
            Microsoft.Office.Interop.Outlook.Application oApp = new Microsoft.Office.Interop.Outlook.Application();
            var mapiNamespace = oApp.GetNamespace("MAPI");

            var calendarFolder =
                mapiNamespace.GetDefaultFolder(Microsoft.Office.Interop.Outlook.OlDefaultFolders.olFolderCalendar);

            var calendarItems = calendarFolder.Items;

            calendarItems.Sort("[Start]", false);
            calendarItems.IncludeRecurrences = true;

            var filter = "[Start] >= \"" + startFilterDate.ToShortDateString() + "\" and [Start] <=\"" + endFilterDate.ToShortDateString() + "\"";

            var item = calendarItems.Find(filter);

            var itemsList = new List<OutlookMeeting>();
            while (item != null)
            {
                var foo = item as Microsoft.Office.Interop.Outlook.AppointmentItem;
                if (foo == null)
                {
                    continue;
                }

                itemsList.Add(new OutlookMeeting(foo.Subject, foo.Body, foo.StartUTC, foo.EndUTC,
                    foo.Recipients.Cast<Recipient>().Select(x => x.Name)));
                item = calendarItems.FindNext();
            }

            return itemsList;
        }
    }

    public class OutlookMeeting
    {
        public readonly string m_subject;
        public readonly string m_body;
        public readonly DateTime m_startUtc;
        public readonly DateTime m_endUtc;
        public readonly TimeSpan m_duration;
        public readonly IEnumerable<string> m_recipients;

        public OutlookMeeting(string subject, string body, DateTime startUtc, DateTime endUtc, IEnumerable<string> recipients)
        {
            m_subject = subject;
            m_body = body;
            m_startUtc = startUtc;
            m_endUtc = endUtc;
            m_recipients = recipients;
        }

        public string Subject
        {
            get { return m_subject; }
        }

        public string Body
        {
            get { return m_body; }
        }

        public DateTime StartUtc
        {
            get { return m_startUtc; }
        }

        public DateTime EndUtc
        {
            get { return m_endUtc; }
        }

        public IEnumerable<string> Recipients
        {
            get { return m_recipients; }
        }

        public TimeSpan Duration { get { return EndUtc - StartUtc; } }
    }
}
