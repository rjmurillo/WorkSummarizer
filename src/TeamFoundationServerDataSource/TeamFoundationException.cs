

using System;

namespace WorkSummarizer.TeamFoundationServerDataSource
{
    public class TeamFoundationException : Exception
    {
         public TeamFoundationException() : base()
        {
        }

        public TeamFoundationException(string message) : base(message)
        {
        }

        public TeamFoundationException(Exception inner) : base("Source control threw something", inner)
        {
        }

        public TeamFoundationException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
