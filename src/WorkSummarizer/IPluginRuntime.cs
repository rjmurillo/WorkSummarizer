using System.Collections;
using System.Collections.Generic;
using Events;
using Renders;

namespace Extensibility
{
    using WorkSummarizer;

    // REVIEW expose service resolution... e.g. to read configuration
    public interface IPluginRuntime
    {
        // REVIEW expose service registration as a service...
        IDictionary<ServiceRegistration, IConfigurationService> ConfigurationServices { get; }

        IDictionary<ServiceRegistration, IEventQueryService> EventQueryServices { get; }

        IDictionary<ServiceRegistration, IRenderEvents> RenderEventServices { get; }
    }
}
