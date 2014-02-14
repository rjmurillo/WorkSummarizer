using System.Collections;
using System.Collections.Generic;
using Events;

namespace Extensibility
{
    // REVIEW expose service resolution... e.g. to read configuration
    public interface IPluginRuntime
    {
        // REVIEW expose service registration as a service...
        IDictionary<ServiceRegistration, IEventQueryService> EventQueryServices { get; }
    }
}
