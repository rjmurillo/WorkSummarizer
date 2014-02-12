﻿using System;
using System.Collections.Generic;
using System.Linq;
using Events.Outlook;
using Events.TeamFoundationServer;
using Events.Yammer;

namespace WorkSummarizer
{
    class Program
    {
        [STAThread] // this is for the Forms authentication dialog for Yammer auth....
        static void Main(string[] args)
        {
            var plugins = new List<Type>();

            // plugins.Add(typeof(OutlookPlugin));
            // plugins.Add(typeof (TeamFoundationServerPlugin));
            // plugins.Add(typeof (YammerPlugin));

            var pluginRuntime = new PluginRuntime();
            pluginRuntime.Start(plugins);

            foreach (var eventQueryServiceRegistration in pluginRuntime.EventQueryServices)
            {
                Console.WriteLine("Querying from event query service: " + eventQueryServiceRegistration.Key);
                eventQueryServiceRegistration.Value.PullEvents(new DateTime(2014, 1, 1), new DateTime(2014, 2, 14));
                Console.WriteLine();
            }

            if (!pluginRuntime.EventQueryServices.Any())
            {
                Console.WriteLine("No event query services registered!");
            }

            Console.ReadKey();
        }
    }
}
