using Desktop_Client.Core.Abstracts;
using Desktop_Client.Core.Tools.Extensions;
using Desktop_Client.Core.ViewModels.Base;
using Desktop_Client.Views.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Models.Client;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace Desktop_Client;

public sealed partial class App : Application
{
    internal static AuthorizeData AuthorizeData { get; set; }

    internal static IHost Host { get; private set; }

    private void Application_Startup(object sender, StartupEventArgs e)
    {
        Host = ConfigureHosting();

        var navService = Host.Services.GetService<INavigationService>();

        navService.SetMainWindow<AuthWindow>();
    }

    private static IHost ConfigureHosting ()
    {
        var builder = new ConfigurationBuilder()
                          .SetBasePath(Directory.GetCurrentDirectory() + "/Resources/")
                          .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        var config = builder.Build();

        var hostBuilder = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder().ConfigureServices(services => {
            services.AddSingleton(typeof(IConfiguration), config);

            services.AddServices();
            services.RegisterViewModels<ViewModel>();
            services.AddViews();
        });

        return hostBuilder.Build();
    }
}