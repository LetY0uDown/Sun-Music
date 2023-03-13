using Desktop_Client.Core.Tools.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace Desktop_Client.Core.Tools;

internal static class ServiceCollectionExtensions
{
    private static void AddServices(IServiceCollection serviceCollection, IEnumerable<Type> services, bool inheritAttributes = false)
    {
        foreach (var service in services)
        {
            var attributes = service.GetCustomAttributes(inheritAttributes);

            var isSingleton = attributes.FirstOrDefault(o => o is SingletonAttribute) is not null;
            var isTransient = attributes.FirstOrDefault(o => o is TransientAttribute) is not null;

            if (isTransient == isSingleton)
            {
                throw new InvalidOperationException($"Cannot determine lifetime of the service {service.FullName}");
            }

            if (isSingleton)
                serviceCollection.AddSingleton(service);

            if (isTransient)
                serviceCollection.AddTransient(service);
        }
    }

    internal static void RegisterViewModels<TViewModel>(this IServiceCollection services)
    {
        var viewModels = Assembly.GetExecutingAssembly()
                                 .GetTypes()
                                 .Where(t => !t.IsAbstract &&
                                              t.IsSubclassOf(typeof(TViewModel))).ToList();

        AddServices(services, viewModels, true);
    }

    internal static void AddPages(this IServiceCollection services)
    {
        var pages = Assembly.GetExecutingAssembly()
                            .GetTypes()
                            .Where(t => t.IsSubclassOf(typeof(Page))).ToList();

        AddServices(services, pages);
    }

    internal static void AddWindows(this IServiceCollection services)
    {
        var pages = Assembly.GetExecutingAssembly()
                            .GetTypes()
                            .Where(t => t.IsSubclassOf(typeof(Window))).ToList();

        AddServices(services, pages);
    }
}