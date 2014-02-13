using System;
using System.Collections.Generic;

namespace DataSources.Outlook
{
    public class OutlookItem
    {
        public readonly string m_subject;
        public readonly string m_body;
        public readonly DateTime m_startUtc;
        public readonly DateTime m_endUtc;
        public readonly TimeSpan m_duration;
        public readonly IEnumerable<string> m_recipients;

        public OutlookItem(string subject, string body, DateTime startUtc, DateTime endUtc, IEnumerable<string> recipients)
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