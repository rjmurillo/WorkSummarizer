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

            var itemsList = new List<OutlookItem>();

            foreach (var item in restrictedItems)
            {
                var mail = item as MailItem;

                if (mail == null)
                {
                    continue;
                }

                itemsList.Add(new OutlookItem(mail.Subject ?? String.Empty, mail.Body ?? String.Empty, mail.CreationTime,
                    mail.CreationTime, mail.Recipients.Cast<Recipient>().Select(x => x.Name)));

                ((Microsoft.Office.Interop.Outlook._MailItem)mail).Close(OlInspectorClose.olDiscard);
            }

            return itemsList;
        }
    }
}


