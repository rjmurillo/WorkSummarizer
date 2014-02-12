using System;
using System.Collections.Generic;
using Events;
using Events.Yammer;
using Extensibility;

namespace WorkSummarizer
{
    public class PluginRuntime : IPluginRuntime
    {
        public PluginRuntime()
        {
            EventQueryServices = new Dictionary<string, IEventQueryService>();
        }
        
        public IDictionary<string, IEventQueryService> EventQueryServices { get; private set; }

        // REVIEW plugin discovery if we were fancy.
        public void Start(IEnumerable<Type> pluginTypes)
        {
            foreach (var pluginType in pluginTypes)
            {
                Console.WriteLine("Loading " + pluginType.FullName);
                Activator.CreateInstance(pluginType, this);
            }
        }
    }
}
