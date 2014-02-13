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
        public IEnumerable<OutlookItem> GetMeetings(DateTime startDateUtc, DateTime endDateUtc)
        {
            var items = GetItems(startDateUtc, endDateUtc, Microsoft.Office.Interop.Outlook.OlDefaultFolders.olFolderCalendar, "[Start]");
            return items.OfType<AppointmentItem>().Select(mail => new OutlookItem(mail.Subject, mail.Body, mail.CreationTime, mail.CreationTime, mail.Recipients.Cast<Recipient>().Select(x => x.Name))).ToList();
        }

        public IEnumerable<OutlookItem> GetEmails(DateTime startDateUtc, DateTime endDateUtc)
        {
            var items = GetItems(startDateUtc, endDateUtc, Microsoft.Office.Interop.Outlook.OlDefaultFolders.olFolderInbox, "[CreationTime]");
            return items.OfType<MailItem>().Select(mail => new OutlookItem(mail.Subject, mail.Body, mail.CreationTime, mail.CreationTime, mail.Recipients.Cast<Recipient>().Select(x => x.Name))).ToList();
        }

        private Microsoft.Office.Interop.Outlook.Items GetItems(DateTime startDateUtc, DateTime endDateUtc, OlDefaultFolders defaultFolders, string filterDateFormat)
        {
            Microsoft.Office.Interop.Outlook.Application oApp = new Microsoft.Office.Interop.Outlook.Application();
            var mapiNamespace = oApp.GetNamespace("MAPI");

            var calendarFolder = mapiNamespace.GetDefaultFolder(defaultFolders);

            var calendarItems = calendarFolder.Items;

            calendarItems.Sort(filterDateFormat, false);
            calendarItems.IncludeRecurrences = true;

            var filter = filterDateFormat + " >= \"" + startDateUtc.ToShortDateString() + "\" and " + filterDateFormat + " <=\"" + endDateUtc.ToShortDateString() + "\"";

            Microsoft.Office.Interop.Outlook.Items restrictedItems = calendarFolder.Items.Restrict(filter);

            return restrictedItems;
        }
    }
}
