using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Outlook;

namespace DataSources.Outlook
{
    public class EmailProvider : IDataPull<OutlookItem>
    {
        public IEnumerable<OutlookItem> PullData(DateTime startDateUtc, DateTime endDateUtc)
        {
            Application oApp = new Application();
            var mapiNamespace = oApp.GetNamespace("MAPI");

            var emailSentFolder =
                mapiNamespace.GetDefaultFolder(OlDefaultFolders.olFolderSentMail);

            var filter = "[SentOn] >= \"" + startDateUtc.ToShortDateString() + "\" and [SentOn] <=\"" +
                         endDateUtc.ToShortDateString() + "\"";

            Items restrictedItems = emailSentFolder.Items.Restrict(filter);

            var itemsList = new List<OutlookItem>();

            foreach (var item in restrictedItems)
            {
                var mail = item as MailItem;

                if (mail == null)
                {
                    continue;
                }

                try
                {
                    if (mail.Class != OlObjectClass.olMobile)
                    {

                        try
                        {
                            itemsList.Add(new OutlookItem(mail.Subject ?? String.Empty, mail.Body ?? String.Empty,
                                mail.CreationTime,
                                mail.CreationTime,
                                mail.Recipients.Cast<Recipient>()
                                    .Select(x => x.Name)
                                    .Union(new[] {mail.Sender != null ? mail.Sender.Name : null})
                                    .ToList()));
                        }
                        catch (COMException e)
                        {
                            if (e.HResult != -2147352573)
                            {
                                throw;
                            }
                        }
                    }
                }
                finally
                {
                    ((Microsoft.Office.Interop.Outlook._MailItem) mail).Close(OlInspectorClose.olDiscard);
                }
            }

            return itemsList;
        }
    }
}


