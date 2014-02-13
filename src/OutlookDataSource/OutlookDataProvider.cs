using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Outlook;

namespace OutlookDataSource
{
    public interface IOutlookDataProvider
    {
        IEnumerable<OutlookItem> GetMeetings(DateTime startDateUtc, DateTime endDateUtc);

        IEnumerable<OutlookItem> GetEmails(DateTime startFilterDate, DateTime endFilterDate);
    }

    public class OutlookDataProvider : IOutlookDataProvider
    {

        public IEnumerable<OutlookItem> GetMeetings(DateTime startFilterDate, DateTime endFilterDate)
        {
            Microsoft.Office.Interop.Outlook.Application oApp = new Microsoft.Office.Interop.Outlook.Application();
            var mapiNamespace = oApp.GetNamespace("MAPI");

            var calendarFolder =
                mapiNamespace.GetDefaultFolder(Microsoft.Office.Interop.Outlook.OlDefaultFolders.olFolderCalendar);

            var calendarItems = calendarFolder.Items;

            calendarItems.Sort("[Start]", false);
            calendarItems.IncludeRecurrences = true;

            var filter = "[Start] >= \"" + startFilterDate.ToShortDateString() + "\" and [Start] <=\"" +
                         endFilterDate.ToShortDateString() + "\"";

            var item = calendarItems.Find(filter);

            var itemsList = new List<OutlookItem>();
            while (item != null)
            {
                var foo = item as Microsoft.Office.Interop.Outlook.AppointmentItem;
                if (foo == null)
                {
                    continue;
                }

                itemsList.Add(new OutlookItem(foo.Subject, foo.Body, foo.StartUTC, foo.EndUTC,
                    foo.Recipients.Cast<Recipient>().Select(x => x.Name)));
                item = calendarItems.FindNext();
            }

            return itemsList;
        }

        public IEnumerable<OutlookItem> GetEmails(DateTime startDateUtc, DateTime endDateUtc)
        {
            Microsoft.Office.Interop.Outlook.Application oApp = new Microsoft.Office.Interop.Outlook.Application();
            var mapiNamespace = oApp.GetNamespace("MAPI");

            var calendarFolder = mapiNamespace.GetDefaultFolder(Microsoft.Office.Interop.Outlook.OlDefaultFolders.olFolderInbox);

            var filter = "[CreationTime] >= \"" + startDateUtc.ToShortDateString() + "\" and [CreationTime] <=\"" +
             endDateUtc.ToShortDateString() + "\"";

            Microsoft.Office.Interop.Outlook.Items restrictedItems = calendarFolder.Items.Restrict(filter);

            return restrictedItems.OfType<MailItem>().Select(mail => new OutlookItem(mail.Subject, mail.Body, mail.CreationTime, mail.CreationTime, mail.Recipients.Cast<Recipient>().Select(x => x.Name))).ToList();
        }
    }
}
