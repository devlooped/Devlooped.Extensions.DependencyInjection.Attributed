﻿// <auto-generated />
#nullable enable
#if DDI_ADDSERVICE
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Configures the registration of a service in an <see cref="IServiceCollection"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    partial class ServiceAttribute : Attribute
    {
        /// <summary>
        /// Annotates the service with the lifetime.
        /// </summary>
        public ServiceAttribute(ServiceLifetime lifetime = ServiceLifetime.Singleton) { }

        /// <summary>
        /// Annotates the service with the given key and lifetime.
        /// </summary>
        public ServiceAttribute(object key, ServiceLifetime lifetime = ServiceLifetime.Singleton) { }
    }
}
#endif