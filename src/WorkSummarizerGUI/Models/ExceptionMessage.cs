using System;

namespace WorkSummarizerGUI.Models
{
    public class ExceptionMessage
    {
        private string m_context;
        private Exception m_exception;

        public ExceptionMessage(Exception exception, string context)
        {
            m_exception = exception;
            m_context = context;
        }

        public string Context
        {
            get
            {
                return this.m_context;
            }
        }

        public Exception Exception
        {
            get
            {
                return this.m_exception;
            }
        }
    }
}
