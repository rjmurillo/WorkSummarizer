﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Office.Interop.Outlook;

namespace DataSources.Outlook
{
    public class MeetingProvider : IDataPull<OutlookItem>
    {
        public IEnumerable<OutlookItem> PullData(DateTime startFilterDate, DateTime endFilterDate)
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
                var appointmentItem = item as Microsoft.Office.Interop.Outlook.AppointmentItem;
                if (appointmentItem == null)
                {
                    item = calendarItems.FindNext();
                    continue;
                }

                itemsList.Add(new OutlookItem(appointmentItem.Subject ?? String.Empty, appointmentItem.Body ?? String.Empty, appointmentItem.StartUTC,
                    appointmentItem.EndUTC,
                    appointmentItem.Recipients.Cast<Recipient>().Select(x => x.Name)));

                ((Microsoft.Office.Interop.Outlook._AppointmentItem)appointmentItem).Close(OlInspectorClose.olDiscard);

                item = calendarItems.FindNext();
            }

            return itemsList;
        }

    }
}