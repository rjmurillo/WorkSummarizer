using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Events.Outlook;
using Events.Yammer;

namespace WorkSummarizer
{
    class Program
    {
        [STAThread] // this is for the Forms authentication dialog for Yammer auth....
        static void Main(string[] args)
        {
            var outlookMeetings = OutlookQueryService.GetMeetings(DateTime.Parse("01/01/2013"), DateTime.Parse("02/11/2014"));
            var foo = outlookMeetings.Select(x => x.Recipients.Count()).Average();

            //var yammer = YammerEventsQueryService.PullUserMessages(DateTime.Parse("01/01/2013"),
            //    DateTime.Parse("02/11/2014"));
            
        }
    }
}
