using Desktop_Client.Core.Tools.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace Desktop_Client.Core.Tools.Extensions;

internal static class ServiceCollectionExtensions
{
    private static void AddServices (IServiceCollection serviceCollection, IEnumerable<Type> services, bool inheritAttributes = false)
    {
        foreach (var service in services) {
            var attributes = service.GetCustomAttributes(inheritAttributes);

            var isSingleton = attributes.Any(o => o is SingletonAttribute);
            var isTransient = attributes.Any(o => o is TransientAttribute);

            if (isTransient == isSingleton)
                throw new InvalidOperationException($"Cannot determine lifetime of the service {service.FullName}");

            if (isSingleton)
                serviceCollection.AddSingleton(service, service);

            if (isTransient)
                serviceCollection.AddTransient(service, service);
        }
    }

    internal static void RegisterViewModels<TViewModel> (this IServiceCollection services) where TViewModel : class
    {
        var viewModels = Assembly.GetExecutingAssembly()
                                 .GetTypes()
                                 .Where(t => !t.IsAbstract &&
                                              t.IsSubclassOf(typeof(TViewModel)));

        AddServices(services, viewModels, true);
    }
    // TODO: Change Page and Window types to interfaces and not break this all
    internal static void AddPages (this IServiceCollection services)
    {
        var pages = Assembly.GetExecutingAssembly()
                            .GetTypes()
                            .Where(t => t.IsSubclassOf(typeof(Page)));

        AddServices(services, pages);
    }

    internal static void AddWindows (this IServiceCollection services)
    {
        var pages = Assembly.GetExecutingAssembly()
                            .GetTypes()
                            .Where(t => t.IsSubclassOf(typeof(Window)));

        AddServices(services, pages);
    }
}