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

        public void Render(string eventType, IEnumerable<Event> events)
        {
            Worksheet sheet = GetSheet(eventType); 
           
            int writingRowNumber = 1;

            foreach (Event evt in events)
            {
                WriteRow(sheet, evt, writingRowNumber++);
            }
            MarkUpColumnWidths(sheet);

            application.Visible = true;
            application.UserControl = true;
        }

        private static void MarkUpColumnWidths(Worksheet sheet)
        {
            var range = sheet.Range["A1"];
            range.EntireColumn.AutoFit();

            range = sheet.Range["B1"];
            range.EntireColumn.AutoFit();
            
            range = sheet.Range["C1"];
            range.EntireColumn.AutoFit();
        }

        private Worksheet GetSheet(string eventType)
        {
            Worksheet sheet;
            if (workbook.Sheets.Count == 1 && string.Equals(((Worksheet)workbook.Sheets[1]).Name, "Sheet1"))
                sheet = workbook.Sheets[1];
            else
                sheet = workbook.Sheets.Add();

            sheet.Name = eventType;

            return sheet;
        }

        private static void WriteRow(Worksheet sheet, Event theEvent, int row)
        {
            sheet.Cells[row, 1] = theEvent.Date;
            sheet.Cells[row, 2] = theEvent.Subject.Text;
            sheet.Cells[row, 3] = theEvent.Text;
        }
    }
}
