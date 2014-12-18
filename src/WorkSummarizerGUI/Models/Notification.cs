using System;
namespace WorkSummarizerGUI.Models
{
    public class Notification
    {
        private readonly string m_message;
        private readonly double m_progress;

        public Notification(double progress, string message)
        {
            m_progress = Math.Min(100, Math.Max(0, progress));
            m_message = message;
        }
        
        public string Message
        {
            get
            {
                return m_message;
            }
        }

        public double Progress
        {
            get
            {
                return m_progress;
            }
        }
    }
}
