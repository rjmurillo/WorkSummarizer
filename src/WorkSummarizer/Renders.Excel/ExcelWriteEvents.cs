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
        
        public void Render(string eventType,DateTime startDateTime, DateTime endDateTime, IEnumerable<Event> events, IDictionary<string, int> weightedTags, IDictionary<string, int> weightedPeople, IEnumerable<string> importantSentences)
        {
            Worksheet sheet = GetSheet(eventType); 
           
            int writingRowNumber = 1;

            writingRowNumber = WriteTags(sheet, writingRowNumber++, weightedTags);

            writingRowNumber = WritePeople(sheet, writingRowNumber++, weightedPeople);

            writingRowNumber = WriteSentences(sheet, writingRowNumber++, importantSentences);

            foreach (var evtGroup in events.GroupBy(v => v.EventType))
            {
                foreach (var evt in evtGroup.OrderByDescending(s => s.Date))
                {
                    WriteRow(sheet, evt, writingRowNumber++);
                }
            }
            MarkUpColumnWidths(sheet);
            
            application.Visible = true;
            application.UserControl = true;
        }

        private int WriteSentences(Worksheet sheet, int row, IEnumerable<string> importantSentences)
        {
            row = WriteHeader(sheet, "Top 10 Sentences", row);
            
            if (importantSentences != null)
            {
                var sentences = (from s in importantSentences select s).Take(10);
                foreach (var sentence in sentences)
                {
                    sheet.Cells[row++, 1] = sentence;
                }
            }
            return ++row;
        }

        private int WritePeople(Worksheet sheet, int row, IDictionary<string, int> weightedPeople)
        {
            return WriteADictionary("Top 10 People", sheet, row, weightedPeople);
        }

        private int WriteTags(Worksheet sheet, int row, IDictionary<string, int> weightedTags)
        {
            return WriteADictionary("Top key words", sheet, row, weightedTags);
        }

        private int WriteHeader(Worksheet sheet, string topWhat, int row)
        {
            sheet.Cells[row, 1] = topWhat;
            var font = sheet.Range["A" + row].Font;
            font.Bold = true;
            return ++row;
        }

        private int WriteADictionary(string title, Worksheet sheet, int row, IDictionary<string, int> keyValue)
        {
            row = WriteHeader(sheet, title, row);

            if (keyValue != null)
            {
                if (keyValue != null)
                {
                    var ss = (from abc in keyValue select abc).OrderByDescending(s => s.Value).Take(10);

                    foreach (var aRow in ss)
                    {
                        sheet.Cells[row, 1] = aRow.Key;
                        sheet.Cells[row++, 2] = aRow.Value;
                    }
                }
            }
            return ++row;
        }

        private Worksheet GetSheet(string eventType)
        {
            if (application == null)
            {
                application = new Application();               
            }
            workbook = application.Workbooks.Add();

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
            sheet.Cells[row, 2] = theEvent.EventType;
            sheet.Cells[row, 3] = string.Empty;//theEvent.Subject.Text;
            sheet.Cells[row, 4] = theEvent.Text;
        }

        private static void MarkUpColumnWidths(Worksheet sheet)
        {
            var range = sheet.Range["A1"];
            range.EntireColumn.ColumnWidth = 25;

            range = sheet.Range["B1"];
            range.EntireColumn.AutoFit();

            range = sheet.Range["C1"];
            range.EntireColumn.AutoFit();

            range = sheet.Range["D1"];
            range.EntireColumn.ColumnWidth = 250;
        }
    }
}
