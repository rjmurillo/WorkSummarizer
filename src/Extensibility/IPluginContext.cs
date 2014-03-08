using System;
using System.Collections.Generic;

namespace Extensibility
{
    public interface IPluginContext
    {
        void RegisterService(ISet<Type> serviceTypes, object service, ServiceRegistration registration);
    }

    public static class IPluginContextExtensions
    {
        public static void RegisterService<TService>(
            this IPluginContext pluginContext, 
            object service,
            ServiceRegistration registration)
        {
            pluginContext.RegisterService(new HashSet<Type> {typeof (TService)}, service, registration);
        }
    }
}
