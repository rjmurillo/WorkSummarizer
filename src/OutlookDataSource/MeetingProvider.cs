using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Outlook;

namespace DataSources.Outlook
{
    public class MeetingProvider : IDataPull<OutlookItem>
    {
        public IEnumerable<OutlookItem> PullData(DateTime startFilterDate, DateTime endFilterDate)
        {
            Application oApp = new Microsoft.Office.Interop.Outlook.Application();
            var mapiNamespace = oApp.GetNamespace("MAPI");

            var calendarFolder =
                mapiNamespace.GetDefaultFolder(Microsoft.Office.Interop.Outlook.OlDefaultFolders.olFolderCalendar);

            var calendarItems = calendarFolder.Items;

            calendarItems.Sort("[Start]", false);
            calendarItems.IncludeRecurrences = true;

            var filter = "[Start] >= \"" + startFilterDate.ToShortDateString() + "\" and [Start] <=\"" +
                         endFilterDate.ToString(CultureInfo.CurrentUICulture) + "\"";

            var item = calendarItems.Find(filter);

            var itemsList = new List<OutlookItem>();
            while (item != null)
            {
                var appointmentItem = item as Microsoft.Office.Interop.Outlook.AppointmentItem;
                if (appointmentItem == null)
                {
                    //((Microsoft.Office.Interop.Outlook._AppointmentItem)item).Close(OlInspectorClose.olDiscard);
                    item = calendarItems.FindNext();

                    continue;
                }

                string subject = null;
                try
                {
                    subject = appointmentItem.Subject;
                }
                catch (COMException)
                {
                }


                string body = null;
                try
                {
                    body = appointmentItem.Body;
                }
                catch (COMException)
                {
                }

                itemsList.Add(new OutlookItem(
                    subject ?? string.Empty, 
                    body ?? string.Empty, 
                    appointmentItem.StartUTC,
                    appointmentItem.EndUTC,
                    appointmentItem.Recipients.Cast<Recipient>().Select(x => x.Name)));

                try
                {
                    ((Microsoft.Office.Interop.Outlook._AppointmentItem) appointmentItem).Close(
                        OlInspectorClose.olDiscard);
                }
                catch (OutOfMemoryException)
                {
                }

                item = calendarItems.FindNext();
            }

            return itemsList;
        }

    }
}