using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using Events;
using Microsoft.Office.Interop.Excel;

namespace Renders.Excel
{
    public class ExcelWriteEvents : IRenderEvents
    {
        private Application application;
        private Workbook workbook;
        
        public void Render(string eventType, IEnumerable<Event> events, IDictionary<string, int> weightedTags, IDictionary<string, int> weightedPeople, IEnumerable<string> importantSentences)
        {
            Worksheet sheet = GetSheet(eventType); 
           
            int writingRowNumber = 1;

            WriteTags(sheet, writingRowNumber++, weightedTags);

            WritePeople(sheet, writingRowNumber++, weightedPeople);

            WriteSentences(sheet, writingRowNumber++, importantSentences);

            foreach (Event evt in events)
            {
                WriteRow(sheet, evt, writingRowNumber++);
            }
            MarkUpColumnWidths(sheet);
            
            application.Visible = true;
            application.UserControl = true;
        }

        private void WriteSentences(Worksheet sheet, int row, IEnumerable<string> importantSentences)
        {
            sheet.Cells[row++, 1] = "Top 10 Sentences";
            var font = sheet.Range["A" + row].Font;
            font.Bold = true;

            if (importantSentences != null)
            {
                var sentences = (from s in importantSentences select s).Take(10);
                foreach (var sentence in sentences)
                {
                    sheet.Cells[row++, 1] = sentence;
                }
            }
            row++;
        }

        private void WritePeople(Worksheet sheet, int row, IDictionary<string, int> weightedPeople)
        {
            sheet.Cells[row++, 1] = "Top 10 People";
            var font = sheet.Range["A" + row].Font;
            font.Bold = true;

            if (weightedPeople != null)
            {
                var ss = (from abc in weightedPeople select abc).OrderByDescending(s => s.Value).Take(10);

                foreach (var aRow in ss)
                {
                    sheet.Cells[row, 1] = aRow.Key;
                    sheet.Cells[row++, 2] = aRow.Value;
                }
            }
            row++;
        }

        private void WriteTags(Worksheet sheet, int row, IDictionary<string, int> weightedTags)
        {
            sheet.Cells[row, 1] = "Top key words";
            var font = sheet.Range["A" + row].Font;
            font.Bold = true;
            row ++;

            if (weightedTags != null)
            {
                if (weightedTags != null)
                {
                    var ss = (from abc in weightedTags select abc).OrderByDescending(s => s.Value).Take(10);

                    foreach (var aRow in ss)
                    {
                        sheet.Cells[row, 1] = aRow.Key;
                        sheet.Cells[row++, 2] = aRow.Value;
                    }
                }
            }
            row++;
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
            if (application == null)
            {
                application = new Application();
                workbook = application.Workbooks.Add();
            }

            Worksheet sheet;
            if (workbook.Sheets.Count == 1 && string.Equals(((Worksheet)workbook.Sheets[1]).Name, "Sheet1", StringComparison.OrdinalIgnoreCase))
                sheet = workbook.Sheets[1];
            else
                sheet = workbook.Sheets.Add();

            try
            {
                sheet.Name = eventType;
            }
            catch (Exception)
            {
                // don't do the neame change, but the sheet is still valid and can be used
            }
            
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
