using System;
using System.Collections.Generic;
using Events;
using Microsoft.Office.Interop.Excel;

namespace Renders.Excel
{
    public class ExcelWriteEvents : IRenderEvents
    {
        readonly Application application = new Application();
        private readonly Workbook workbook;

        public ExcelWriteEvents()
        {
            workbook = application.Workbooks.Add();
        }

        public void WriteOut(string eventType, IEnumerable<Event> events)
        {   
            Worksheet sheet = workbook.Sheets.Add();
            sheet.Name = eventType;

            int writingRowNumber = 1;

            foreach (Event evt in events)
            {
                WriteRow(sheet, evt, writingRowNumber++);
            }
            

            application.Visible = true;
            application.UserControl = true;
        }

        private static void WriteRow(Worksheet sheet, Event theEvent, int row)
        {
            sheet.Cells[row, 1] = theEvent.Date;
            sheet.Cells[row, 2] = theEvent.Subject.Text;
            sheet.Cells[row, 3] = theEvent.Text;
        }
    }
}
