using Desktop_Client.Core.Abstracts;
using Desktop_Client.Core.Tools;
using Desktop_Client.Core.ViewModels.Base;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Desktop_Client.Core.Services;

internal sealed class NavigationService : INavigationService
{
    private NavigationViewModel _navigationVM;

    public INavigationWindow MainWindow { get; private set; }

    public void SetCurrentPage<T> (params (string Name, object Value)[] parameters) where T : INavigationPage
    {
        var page = App.Host.Services.GetRequiredService<T>();

        PropertiesSetter.SetParameters(page, parameters);

        SetPage(page);
    }

    public void SetCurrentPage<T> () where T : INavigationPage
    {
        SetPage(App.Host.Services.GetRequiredService<T>());
    }

    public void SetMainWindow<T> (params (string Name, object Value)[] parameters) where T : INavigationWindow
    {
        var window = App.Host.Services.GetRequiredService<T>();

        PropertiesSetter.SetParameters(window, parameters);

        SetWindow(window);
    }

    public void SetMainWindow<T> () where T : INavigationWindow
    {
        SetWindow(App.Host.Services.GetRequiredService<T>());
    }

    public void SetViewModel (NavigationViewModel viewModel)
    {
        _navigationVM = viewModel;
    }

    void SetWindow (INavigationWindow window)
    {
        MainWindow?.Hide();

        MainWindow = window;

        if (MainWindow is null) {
            InfoBox.Show("Ошибка. Окно не инициализировано, обратитесь к разработчику");
            return;
        }

        MainWindow.Display();
    }

    void SetPage (INavigationPage page)
    {
        if (_navigationVM is null)
            throw new NullReferenceException("Navigation view model is null");

        _navigationVM.CurrentPage = page;
    }
}