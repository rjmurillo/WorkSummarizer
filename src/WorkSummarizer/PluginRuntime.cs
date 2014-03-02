using System;
using System.Collections.Generic;
using System.Diagnostics;
using Events;
using Extensibility;
using Renders;

namespace WorkSummarizer
{
    public class PluginRuntime : IPluginRuntime
    {
        public PluginRuntime()
        {
            ConfigurationServices = new Dictionary<ServiceRegistration, IConfigurationService>();
            EventQueryServices = new Dictionary<ServiceRegistration, IEventQueryService>();
            RenderEventServices = new Dictionary<ServiceRegistration, IRenderEvents>();
        }

        public IDictionary<ServiceRegistration, IConfigurationService> ConfigurationServices { get; private set; }

        public IDictionary<ServiceRegistration, IEventQueryService> EventQueryServices { get; private set; }
        
        public IDictionary<ServiceRegistration, IRenderEvents> RenderEventServices { get; private set; }
        
        // REVIEW plugin discovery if we were fancy.
        public void Start(IEnumerable<Type> pluginTypes)
        {
            foreach (var pluginType in pluginTypes)
            {
                Console.WriteLine("Loading " + pluginType.FullName);
                try
                {
                    Activator.CreateInstance(pluginType, this);
                }
                catch (Exception ex)
                {
                    Debug.Fail(ex.ToString());
                    Trace.WriteLine(ex);
                }
            }
        }
    }
}
