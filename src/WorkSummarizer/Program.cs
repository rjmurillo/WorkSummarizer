using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Events;
using Events.CodeFlow;
using Events.Connect;
using Events.Kudos;
using Events.ManicTime;
using Events.Outlook;
using Events.TeamFoundationServer;
using Events.Yammer;
using Microsoft.Office.Interop.Excel;
using Processing.Text;
using Renders;
using Renders.Console;
using Renders.Excel;
using Renders.HTML;

namespace WorkSummarizer
{
    class Program
    {
        [STAThread] // this is for the Forms authentication dialog for Yammer auth....
        static void Main(string[] args)
        {
            var plugins = new List<Type>();

            //plugins.Add(typeof(FakeEventsPlugin));
            plugins.Add(typeof(CodeFlowPlugin));
            // plugins.Add(typeof(ConnectPlugin));
            plugins.Add(typeof(KudosPlugin));
            // plugins.Add(typeof(ManicTimePlugin));
            plugins.Add(typeof(OutlookPlugin));
            plugins.Add(typeof(TeamFoundationServerPlugin));
            plugins.Add(typeof(YammerPlugin));
            
            //plugins.Add(typeof(ConsoleRenderPlugin));
            //plugins.Add(typeof(ExcelRenderPlugin));
            plugins.Add(typeof(HtmlRenderPlugin));

            var pluginRuntime = new PluginRuntime();
            pluginRuntime.Start(plugins);

            foreach (var eventQueryServiceRegistration in pluginRuntime.EventQueryServices)
            {
                var sb = new StringBuilder();
                Console.WriteLine("Querying from event query service: " + eventQueryServiceRegistration.Key);
                var evts = eventQueryServiceRegistration.Value.PullEvents(new DateTime(2014, 1, 1), new DateTime(2014, 2, 14), Environment.UserName).ToList();

                foreach (var evt in evts)
                {
                    sb.Append(String.Format(" {0} {1} ", evt.Subject.Text.Replace("\n", String.Empty).Replace("\r", String.Empty), evt.Text.Replace("\n", String.Empty).Replace("\r", String.Empty)));
                }

                var textProc = new TextProcessor();
                var peopleProc = new PeopleProcessor();
               
                IDictionary<string, int> weightedTags = textProc.GetNouns(sb.ToString());
                IEnumerable<string> importantSentences = textProc.GetImportantSentences(sb.ToString());
                IDictionary<string, int> weightedPeople = peopleProc.GetTeam(evts);

                foreach (IRenderEvents render in pluginRuntime.RenderEventServices.Values)
                {
                    render.Render(eventQueryServiceRegistration.Key.Id, evts, weightedTags, weightedPeople, importantSentences);
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
