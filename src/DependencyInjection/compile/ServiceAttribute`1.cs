﻿// <auto-generated />
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Configures the registration of a keyed service in an <see cref="IServiceCollection"/>.
    /// Requires v8 or later of Microsoft.Extensions.DependencyInjection package.
    /// </summary>
    /// <typeparam name="TKey">Type of service key.</typeparam>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    [Obsolete("Use ServiceAttribute(object key, ServiceLifetime lifetime) instead.")]
    partial class ServiceAttribute<TKey> : Attribute
    {
        /// <summary>
        /// Annotates the service with the lifetime.
        /// </summary>
        public ServiceAttribute(TKey key, ServiceLifetime lifetime = ServiceLifetime.Singleton) { }
    }
}