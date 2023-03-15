using Desktop_Client.Core.ViewModels.Base;

namespace Desktop_Client.Core.Abstracts;

public interface INavigationService
{
    void SetCurrentPage<T>(params (string Name, object Value)[] parameters) where T : INavigationPage;

    void SetCurrentPage<T>() where T : INavigationPage;

    void SetMainWindow<T>(params (string Name, object Value)[] parameters) where T : INavigationWindow;

    void SetMainWindow<T>() where T : INavigationWindow;

    void SetViewModel(NavigationViewModel viewModel);
}