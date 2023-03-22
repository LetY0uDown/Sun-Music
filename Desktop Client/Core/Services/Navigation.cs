using Desktop_Client.Core.Abstracts;
using Desktop_Client.Core.Tools;
using Desktop_Client.Core.Tools.Attributes;
using Desktop_Client.Core.ViewModels.Base;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Desktop_Client.Core.Services;

[HasLifetime(Lifetime.Singleton)]
internal sealed class NavigationService : INavigationService
{
    private NavigationViewModel _navigationVM;

    public INavigationWindow MainWindow { get; private set; }

    public async Task SetCurrentPage<T> (params (string Name, object Value)[] parameters) where T : INavigationPage
    {
        var page = App.Host.Services.GetRequiredService<T>();

        PropertiesSetter.SetParameters(page, parameters);

        await SetPage(page);
    }

    public async Task SetCurrentPage<T> () where T : INavigationPage
    {
        await SetPage(App.Host.Services.GetRequiredService<T>());
    }

    public async Task SetMainWindow<T> (params (string Name, object Value)[] parameters) where T : INavigationWindow
    {
        var window = App.Host.Services.GetRequiredService<T>();

        PropertiesSetter.SetParameters(window, parameters);

        await SetWindow(window);
    }

    public async Task SetMainWindow<T> () where T : INavigationWindow
    {
        await SetWindow(App.Host.Services.GetRequiredService<T>());
    }

    public async Task DisplayWindow<T>(params (string Name, object Value)[] parameters) where T : INavigationWindow
    {
        var window = App.Host.Services.GetRequiredService<T>();

        PropertiesSetter.SetParameters (window, parameters);

        await window.Display();
    }

    public async Task DisplayWindow<T>() where T : INavigationWindow
    {
        var window = App.Host.Services.GetRequiredService<T>();

        await window.Display();
    }

    public void SetViewModel (NavigationViewModel viewModel)
    {
        _navigationVM = viewModel;
    }

    async Task SetWindow (INavigationWindow window)
    {
        MainWindow?.Hide();

        MainWindow = window;

        if (MainWindow is null) {
            InfoBox.Show("Ошибка. Окно не инициализировано, обратитесь к разработчику");
            return;
        }

        await MainWindow.Display();
    }

    async Task SetPage (INavigationPage page)
    {
        if (_navigationVM is null)
            throw new NullReferenceException("Navigation view model is null");

        _navigationVM.CurrentPage = page;
        await _navigationVM.CurrentPage.Display();
    }
}