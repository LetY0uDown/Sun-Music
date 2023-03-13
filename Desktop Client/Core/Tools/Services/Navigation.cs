using Desktop_Client.Core.Abstracts;
using Desktop_Client.Core.ViewModels.Base;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Desktop_Client.Core.Tools.Services;

internal sealed class NavigationService : INavigationService
{
    private NavigationViewModel _navigationVM;

    public INavigationWindow MainWindow { get; private set; }

    public void SetCurrentPage<T>() where T : INavigationPage
    {
        if (_navigationVM is null) 
            throw new NullReferenceException("Navigation view model is null");

        _navigationVM.CurrentPage = App.Host.Services.GetRequiredService<T>();
    }

    public void SetMainWindow<T>() where T : INavigationWindow
    {
        MainWindow?.Hide();

        MainWindow = App.Host.Services.GetRequiredService<T>();

        if (MainWindow is null) {
            InfoBox.Show("Ошибка. Окно не инициализировано, обратитесь к разработчику");
            return;
        }

        MainWindow.Display();
    }

    public void SetViewModel(NavigationViewModel viewModel)
    {
        _navigationVM = viewModel;
    }
}