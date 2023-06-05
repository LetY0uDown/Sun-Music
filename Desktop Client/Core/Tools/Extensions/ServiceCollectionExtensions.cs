using Desktop_Client.Core.Abstracts;
using Desktop_Client.Core.Tools.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Desktop_Client.Core.Tools.Extensions;

internal static class ServiceCollectionExtensions
{
    private static Type[] GetTypes()
    {
        return Assembly.GetExecutingAssembly()
                       .GetTypes();
    }

    private static Type GetBaseType(Type type)
    {
        var interfaces = type.GetInterfaces();

        foreach (var i in interfaces)
        {
            var attributes = i.GetCustomAttributes();

            if (attributes.Any(a => a is BaseTypeAttribute))
            {
                return i;
            }
        }

        throw new InvalidOperationException($"Can not get base type for {type.FullName}");
    }

    private static Lifetime DetermineLifetime(Type service, bool inheritAttributes)
    {
        var lifetimeAttribute = service.GetCustomAttributes(inheritAttributes)
                                       .FirstOrDefault(s => s is LifetimeAttribute)
                                       as LifetimeAttribute;

        if (lifetimeAttribute is null)
            throw new InvalidOperationException($"Cannot determine lifetime of the service {service.FullName}");

        return lifetimeAttribute.Lifetime;
    }

    private static void AddServiceTypesAsBaseTypes(IServiceCollection serviceCollection, IEnumerable<Type> services, bool inheritAttributes)
    {
        foreach (var service in services)
        {
            var lifetime = DetermineLifetime(service, inheritAttributes);

            var baseType = GetBaseType(service);

            if (lifetime == Lifetime.Singleton)
            {
                serviceCollection.AddSingleton(baseType, service);
                continue;
            }

            if (lifetime == Lifetime.Transient)
            {
                serviceCollection.AddTransient(baseType, service);
            }
        }
    }

    private static void AddServiceTypesDirectly(IServiceCollection serviceCollection, IEnumerable<Type> services, bool inheritAttributes)
    {
        foreach (var service in services)
        {
            var lifetime = DetermineLifetime(service, inheritAttributes);

            if (lifetime == Lifetime.Singleton)
            {
                serviceCollection.AddSingleton(service, service);
                continue;
            }

            if (lifetime == Lifetime.Transient)
            {
                serviceCollection.AddTransient(service, service);
            }
        }
    }

    private static void AddServiceTypes(IServiceCollection serviceCollection, IEnumerable<Type> services, bool useBaseType, bool inheritAttributes = false)
    {
        if (useBaseType)
            AddServiceTypesAsBaseTypes(serviceCollection, services, inheritAttributes);
        else
            AddServiceTypesDirectly(serviceCollection, services, inheritAttributes);
    }

    internal static void AddServices(this IServiceCollection serviceCollection)
    {
        var services = GetTypes().Where(t => t.IsClass &&
                                             t.IsAssignableTo(typeof(IService)));

        AddServiceTypes(serviceCollection, services, useBaseType: true);
    }

    internal static void RegisterViewModels<TViewModel>(this IServiceCollection services)
    {
        var viewModels = GetTypes().Where(t => t.IsClass && !t.IsAbstract &&
                                               t.IsAssignableTo(typeof(TViewModel)));

        AddServiceTypes(services, viewModels, useBaseType: false, inheritAttributes: true);
    }

    internal static void AddViews(this IServiceCollection serviceCollection)
    {
        var pages = GetTypes().Where(t => t.IsClass &&
                                          t.IsAssignableTo(typeof(IView)));

        AddServiceTypes(serviceCollection, pages, useBaseType: false);
    }
}