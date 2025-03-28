namespace TestCommon;

using Common;

using Microsoft.Extensions.DependencyInjection;

using Moq.AutoMock;

using System;
using System.Collections.Generic;
using System.Diagnostics;

public class AutoMockerWithKeyedServiceSupport : AutoMocker, IKeyedServiceProvider, IServiceProviderIsKeyedService
{
    private readonly Dictionary<Type, Dictionary<object, object>> _keyedServices = [];

    public object? GetKeyedService(Type serviceType, object? serviceKey)
    {
        Throws.IfNull(serviceKey);

        return _keyedServices.TryGetValue(serviceType, out var services)
            ? services.TryGetValue(serviceKey, out var service) ? service : null
            : null;
    }

    public object GetRequiredKeyedService(Type serviceType, object? serviceKey)
    {
        Throws.IfNull(serviceKey);

        if (_keyedServices.TryGetValue(serviceType, out var services))
        {
            if (services.TryGetValue(serviceKey, out var service))
            {
                return service;
            }
        }

        throw new InvalidOperationException("No service of type " + serviceType);
    }

    public void AddKeyedService<T>(object serviceKey, T service) where T : notnull => AddKeyedService(typeof(T), serviceKey, service);
    public void AddKeyedService(Type serviceType, object serviceKey, object service)
    {
        Debug.WriteLineIf(ResolvedObjects.ContainsKey(serviceType), $"**WARNING** Service of type {serviceType} has already been resolved in the container and is now being registered with a Key!");

        if (!_keyedServices.TryGetValue(serviceType, out var services))
        {
            services = [];
            _keyedServices[serviceType] = services;
        }

        services[serviceKey] = service;
    }

    public bool IsKeyedService(Type serviceType, object? serviceKey) => _keyedServices.TryGetValue(serviceType, out var services) && services.ContainsKey(Throws.IfNull(serviceKey));

    public bool IsService(Type serviceType) => _keyedServices.ContainsKey(serviceType) || ResolvedObjects.ContainsKey(serviceType);
}
