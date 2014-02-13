using System;

namespace DataSources.Kudos
{
    public class KudosPost
    {
        public DateTime CreatedUtcTime { get; set; }
        public string Message { get; set; }
        public string SenderAlias { get; set; }
    }
}
