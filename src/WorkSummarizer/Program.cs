﻿using System;
using System.Collections.Generic;
using System.Linq;
using Events;
using Events.Outlook;
using Events.TeamFoundationServer;
using Events.Yammer;
using Microsoft.Office.Interop.Excel;

namespace WorkSummarizer
{
    class Program
    {
        [STAThread] // this is for the Forms authentication dialog for Yammer auth....
        static void Main(string[] args)
        {
            var plugins = new List<Type>();

            plugins.Add(typeof(OutlookPlugin));
            // plugins.Add(typeof (TeamFoundationServerPlugin));
            // plugins.Add(typeof (YammerPlugin));

            var pluginRuntime = new PluginRuntime();
            pluginRuntime.Start(plugins);

            Application application = new Application();
            Workbook workbook = application.Workbooks.Add();
            Worksheet sheet = workbook.ActiveSheet;
            int writingRowNumber = 1;
            foreach (var eventQueryServiceRegistration in pluginRuntime.EventQueryServices)
            {
                Console.WriteLine("Querying from event query service: " + eventQueryServiceRegistration.Key);
                
                foreach(var evt in eventQueryServiceRegistration.Value.PullEvents(new DateTime(2014, 1, 1), new DateTime(2014, 2, 14)))
                {
                    //Console.WriteLine("{0} {1}: {2}...", evt.Date, evt.Subject, evt.Text.Substring(0, Math.Min(evt.Text.Length, 30)).Replace("\n", String.Empty));
                    WriteRow(sheet, evt, writingRowNumber++);
                }
                
                Console.WriteLine();
            }

            if (!pluginRuntime.EventQueryServices.Any())
            {
                Console.WriteLine("No event query services registered!");
            }

            application.Visible = true;
            application.UserControl = true;

            Console.ReadKey();

            /* example: grab TFS workitems and build a graph
            var tfs = new TfsData();
            //var changesets = tfs.PullChangesets(new Uri("http://vstfcodebox:8080/tfs/Engineering_Excellence"), "EE_Engineering", DateTime.Parse("1/1/2014"), DateTime.Today);
            var workItems = tfs.PullWorkItemsThatChanged(new Uri("http://vstfcodebox:8080/tfs/Engineering_Excellence"), "EE_Engineering", DateTime.Parse("1/22/2014"), DateTime.Today);
            var graph = TfsHelper.BuildWorkItemGraph(workItems);
            */
        }

        private static void WriteRow(Worksheet sheet, Event theEvent, int row)
        {
            sheet.Cells[row, 1] = theEvent.Date;
            sheet.Cells[row, 2] = theEvent.Subject.Text;
            sheet.Cells[row, 3] = theEvent.Text;
        }

    }
}
