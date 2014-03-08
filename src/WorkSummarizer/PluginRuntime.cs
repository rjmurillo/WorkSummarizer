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
                    var pluginContext = new PluginContext(pluginType.FullName, this);
                    Activator.CreateInstance(pluginType, pluginContext);
                }
                catch (Exception ex)
                {
                    Debug.Fail(ex.ToString());
                    Trace.WriteLine(ex);
                }
            }
        }
    }

    internal class PluginContext : IPluginContext
    {
        private readonly string id;
        private readonly IPluginRuntime pluginRuntime;

        public PluginContext(string id, IPluginRuntime pluginRuntime)
        {
            this.id = id;
            this.pluginRuntime = pluginRuntime;
        }

        public string Id
        {
            get { return this.id; }
        }

        public void RegisterService(ISet<Type> serviceTypes, object service, ServiceRegistration registration)
        {
            foreach (var serviceType in serviceTypes)
            {
                if (!serviceType.IsInstanceOfType(service))
                {
                    throw new InvalidOperationException("Invalid service registration type mapping");
                }

                // TODO revise "shared" interfaces - probably use FullName keys?
                if (serviceType.IsAssignableFrom(typeof (IEventQueryService)))
                {
                    pluginRuntime.EventQueryServices[registration] = service as IEventQueryService;
                }

                if (serviceType.IsAssignableFrom(typeof(IRenderEvents)))
                {
                    pluginRuntime.RenderEventServices[registration] = service as IRenderEvents;
                }

                if (service is IConfigurable)
                {
                    registration.IsConfigurable = true;
                    var configurableService = service as IConfigurable;
                    pluginRuntime.ConfigurationServices[registration] = new DefaultConfigurationService(configurableService.Settings);
                }
            }
        }
    }
}
