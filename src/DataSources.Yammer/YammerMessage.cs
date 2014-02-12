using System;

namespace DataSources.Yammer
{
    public class YammerMessage
    {
        private readonly string m_body;
        private readonly string m_sender;
        private readonly DateTime m_createdUtc;

        public YammerMessage(string body, string sender, DateTime createdUtc)
        {
            m_body = body;
            m_sender = sender;
            m_createdUtc = createdUtc;
        }

        public string Body
        {
            get { return m_body; }
        }

        public DateTime CreatedAt 
        {
            get { return m_createdUtc; }
        }

        public string Sender
        {
            get { return m_sender; }
        }
    }
}