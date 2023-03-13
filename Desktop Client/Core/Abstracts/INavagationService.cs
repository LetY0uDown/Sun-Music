using Desktop_Client.Core.ViewModels.Base;
using System.Windows;
using System.Windows.Controls;

namespace Desktop_Client.Core.Abstracts;

public interface INavigationService
{
    void SetCurrentPage<T>() where T : Page;

    void SetMainWindow<T>() where T : Window;

    void SetViewModel(NavigationViewModel viewModel);
}