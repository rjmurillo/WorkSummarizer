using System;
using Events.Outlook;

namespace WorkSummarizer
{
    class Program
    {
        [STAThread] // this is for the Forms authentication dialog for Yammer auth....
        static void Main(string[] args)
        {
            var meetingQueryService = new OutlookMeetingEventQueryService();
            var events = meetingQueryService.PullEvents(DateTime.Parse("01/01/2014"), DateTime.Parse("02/11/2014"));
            Console.WriteLine(events);

            var emailQueryService = new OutlookEmailEventQueryService();
            var emailEvents = emailQueryService.PullEvents(DateTime.Parse("01/01/2014"), DateTime.Parse("02/11/2014"));
            Console.WriteLine(emailEvents);
        }
    }
}
