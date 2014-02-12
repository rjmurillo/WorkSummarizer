using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Outlook;

namespace OutlookDataSource
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
}
