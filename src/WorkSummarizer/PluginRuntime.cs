using System;
using System.Collections.Generic;
using Events;
using Events.Yammer;
using Extensibility;
using Renders;

namespace WorkSummarizer
{
    public class PluginRuntime : IPluginRuntime
    {
        public PluginRuntime()
        {
            EventQueryServices = new Dictionary<ServiceRegistration, IEventQueryService>();
            RenderEventServices = new Dictionary<ServiceRegistration, IRenderEvents>();
        }

        public IDictionary<ServiceRegistration, IEventQueryService> EventQueryServices { get; private set; }
        
        public IDictionary<ServiceRegistration, IRenderEvents> RenderEventServices { get; private set; }

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
