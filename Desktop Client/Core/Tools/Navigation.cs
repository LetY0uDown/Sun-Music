using Desktop_Client.Core.Abstracts;
using Desktop_Client.Core.ViewModels.Base;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Desktop_Client.Core.Tools;

internal sealed class NavigationService : INavigationService
{
    private NavigationViewModel _navigationVM;

    public void SetCurrentPage<T>() where T : Page
    {
        if (_navigationVM is null) throw new NullReferenceException("Navigation view model is null");

        _navigationVM.CurrentPage = App.Host.Services.GetRequiredService<T>();
    }

    public void SetMainWindow<T>() where T : Window
    {
        Application.Current.MainWindow?.Close();

        Application.Current.MainWindow = App.Host.Services.GetRequiredService<T>();

        Application.Current.MainWindow?.Show();
    }

    public void SetViewModel(NavigationViewModel viewModel)
    {
        _navigationVM = viewModel;
    }
}