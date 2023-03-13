using Desktop_Client.Core.Abstracts;
using Desktop_Client.Core.Tools;
using Desktop_Client.Core.ViewModels.Base;
using Desktop_Client.Views.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Windows;

namespace Desktop_Client;

public partial class App : Application
{
    internal static IHost Host { get; private set; }

    internal static IConfiguration Configuration { get; private set; }

    private static IHost ConfigureHosting()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory() + "/Resources/")
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        Configuration = builder.Build();

        var hostBuilder = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder().ConfigureServices(services =>
        {
            services.AddTransient<IAPIClient, APIClient>();
            services.AddSingleton<INavigationService, NavigationService>();

            services.RegisterViewModels<ViewModel>();
            services.AddWindows();
            services.AddPages();
        });

        return hostBuilder.Build();
    }

    private void Application_Startup(object sender, StartupEventArgs e)
    {
        Host = ConfigureHosting();

        var navService = Host.Services.GetService<INavigationService>();

        navService.SetMainWindow<AuthWindow>();
    }
}