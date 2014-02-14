using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Office.Interop.Outlook;

namespace DataSources.Outlook
{
    public class EmailProvider : IDataPull<OutlookItem>
    {
        public IEnumerable<OutlookItem> PullData(DateTime startDateUtc, DateTime endDateUtc)
        {
            Application oApp = new Application();
            var mapiNamespace = oApp.GetNamespace("MAPI");

            var calendarFolder =
                mapiNamespace.GetDefaultFolder(OlDefaultFolders.olFolderInbox);

            var filter = "[CreationTime] >= \"" + startDateUtc.ToShortDateString() + "\" and [CreationTime] <=\"" +
                         endDateUtc.ToShortDateString() + "\"";

            Items restrictedItems = calendarFolder.Items.Restrict(filter);

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


