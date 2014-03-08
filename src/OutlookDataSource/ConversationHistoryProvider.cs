using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Office.Interop.Outlook;

namespace DataSources.Outlook
{
    public class ConversationHistoryProvider : IDataPull<OutlookItem>
    {
        public IEnumerable<OutlookItem> PullData(DateTime startDateUtc, DateTime endDateUtc)
        {
            Application oApp = new Application();
            var mapiNamespace = oApp.GetNamespace("MAPI");

            Folders folders = mapiNamespace.Folders;

            MAPIFolder conversationFolder = null;
            foreach (MAPIFolder folder in folders)
            {
                var convFolder = FindConversationFolder(folder);

                if (convFolder != null)
                {
                    conversationFolder = convFolder;
                    break;
                }
            }

            if (conversationFolder == null)
            {
                return new List<OutlookItem>();
            }

            var filter = "[CreationTime] >= \"" + startDateUtc.ToShortDateString() + "\" and [CreationTime] <=\"" +
                         endDateUtc.ToShortDateString() + "\"";

            Items restrictedItems = conversationFolder.Items.Restrict(filter);

            var itemsList = new List<OutlookItem>();

            foreach (var item in restrictedItems)
            {
                var mail = item as MailItem;

                if (mail == null)
                {
                    continue;
                }

                itemsList.Add(new OutlookItem(mail.Subject ?? String.Empty, mail.Body ?? String.Empty, mail.CreationTime,
                    mail.CreationTime, mail.Recipients.Cast<Recipient>().Select(x => x.Name).ToList()));

                ((Microsoft.Office.Interop.Outlook._MailItem)mail).Close(OlInspectorClose.olDiscard);
            }

            return itemsList;
        }

        private MAPIFolder FindConversationFolder(MAPIFolder folder)
        {
            if (folder.Name.Equals("Conversation History", StringComparison.OrdinalIgnoreCase))
            {
                return folder;
            }
            else
            {
                Folders childFolders = folder.Folders;
                if (childFolders.Count > 0)
                {
                    foreach (MAPIFolder childFolder in childFolders)
                    {
                        MAPIFolder fol = FindConversationFolder(childFolder);
                        if(fol != null)
                        {
                            return fol;
                        }
                    }
                }

                return null;
            }
        }
    }
}