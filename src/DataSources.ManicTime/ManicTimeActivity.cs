using System;

namespace DataSources.ManicTime
{
    public class ManicTimeActivity
    {
        public string DisplayName { get; set; }
        public string GroupDisplayName { get; set; }
        public string TextData { get; set; }
        public DateTime StartUtcTime { get; set; }
        public DateTime EndUtcTime { get; set; }
    }
}
