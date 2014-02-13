using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Office.Interop.Outlook;

namespace DataSources.Outlook
{
    public class OutlookDataEmailProvider : IDataPull<OutlookItem>
    {
        public IEnumerable<OutlookItem> PullData(DateTime startDateUtc, DateTime endDateUtc)
        {
            Microsoft.Office.Interop.Outlook.Application oApp = new Microsoft.Office.Interop.Outlook.Application();
            var mapiNamespace = oApp.GetNamespace("MAPI");

            var calendarFolder =
                mapiNamespace.GetDefaultFolder(Microsoft.Office.Interop.Outlook.OlDefaultFolders.olFolderInbox);

            var filter = "[CreationTime] >= \"" + startDateUtc.ToShortDateString() + "\" and [CreationTime] <=\"" +
                         endDateUtc.ToShortDateString() + "\"";

            Microsoft.Office.Interop.Outlook.Items restrictedItems = calendarFolder.Items.Restrict(filter);

            return
                restrictedItems.OfType<MailItem>()
                    .Select(
                        mail =>
                            new OutlookItem(mail.Subject ?? String.Empty, mail.Body ?? String.Empty, mail.CreationTime,
                                mail.CreationTime, mail.Recipients.Cast<Recipient>().Select(x => x.Name)))
                    .ToList();
        }
    }
}


