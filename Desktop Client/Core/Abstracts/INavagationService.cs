using Desktop_Client.Core.Tools.Attributes;
using Desktop_Client.Core.ViewModels.Base;

namespace Desktop_Client.Core.Abstracts;

[Singleton]
public interface INavigationService
{
    void SetCurrentPage<T>() where T : INavigationPage;

    void SetMainWindow<T>() where T : INavigationWindow;

    void SetViewModel(NavigationViewModel viewModel);
}