using System;
using System.Collections.Generic;
using Events;
using Microsoft.Office.Interop.Excel;

namespace Renders.Excel
{
    public class ExcelWriteEvents : IRenderEvents
    {
        public void WriteOut(IEnumerable<Event> events)
        {
            Application application = new Application();
            Workbook workbook = application.Workbooks.Add();
            Worksheet sheet = workbook.ActiveSheet;
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
