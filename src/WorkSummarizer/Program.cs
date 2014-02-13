using System;
using System.Collections.Generic;
using System.Linq;
using Events;
using Events.CodeFlow;
using Events.Connect;
using Events.Kudos;
using Events.ManicTime;
using Events.Outlook;
using Events.TeamFoundationServer;
using Events.Yammer;
using Microsoft.Office.Interop.Excel;
using Renders;
using Renders.Console;
using Renders.Excel;

namespace WorkSummarizer
{
    class Program
    {
        [STAThread] // this is for the Forms authentication dialog for Yammer auth....
        static void Main(string[] args)
        {
            var plugins = new List<Type>();

            //plugins.Add(typeof(FakeEventsPlugin));
            // plugins.Add(typeof(CodeFlowPlugin));
            // plugins.Add(typeof(ConnectPlugin));
            // plugins.Add(typeof(KudosPlugin));
            // plugins.Add(typeof(ManicTimePlugin));
             plugins.Add(typeof(OutlookPlugin));
            // plugins.Add(typeof(TeamFoundationServerPlugin));
            // plugins.Add(typeof(YammerPlugin));

            var pluginRuntime = new PluginRuntime();
            pluginRuntime.Start(plugins);

            var renders = new List<IRenderEvents>();
            renders.Add(new ExcelWriteEvents());
            renders.Add(new ConsoleWriteEvents());
            
            foreach (var eventQueryServiceRegistration in pluginRuntime.EventQueryServices)
            {
                Console.WriteLine("Querying from event query service: " + eventQueryServiceRegistration.Key);
                var evts = eventQueryServiceRegistration.Value.PullEvents(new DateTime(2014, 1, 1), new DateTime(2014, 2, 14));
               
                foreach (IRenderEvents render in renders)
                {
                    render.WriteOut(evts);
                }
                
                Console.WriteLine();
            }

            if (!pluginRuntime.EventQueryServices.Any())
            {
                Console.WriteLine("No event query services registered!");
            }

            Console.WriteLine("Done.");
            Console.ReadKey(true);

            /* example: grab TFS workitems and build a graph
            var tfs = new TfsData();
            //var changesets = tfs.PullChangesets(new Uri("http://vstfcodebox:8080/tfs/Engineering_Excellence"), "EE_Engineering", DateTime.Parse("1/1/2014"), DateTime.Today);
            var workItems = tfs.PullWorkItemsThatChanged(new Uri("http://vstfcodebox:8080/tfs/Engineering_Excellence"), "EE_Engineering", DateTime.Parse("1/22/2014"), DateTime.Today);
            var graph = TfsHelper.BuildWorkItemGraph(workItems);
            */
        }

    }
}
