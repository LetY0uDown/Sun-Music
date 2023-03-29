using Desktop_Client.Core.Tools.Attributes;
using Desktop_Client.Core.ViewModels.Base;
using System.Threading.Tasks;

namespace Desktop_Client.Core.Abstracts;

[BaseType]
public interface INavigationService : IService
{
    Task SetCurrentPage<T>(params (string Name, object Value)[] parameters) where T : INavigationPage;

    Task SetCurrentPage<T>() where T : INavigationPage;

    Task SetMainWindow<T>(params (string Name, object Value)[] parameters) where T : INavigationWindow;

    Task SetMainWindow<T>() where T : INavigationWindow;
    
    Task DisplayWindow<T>(params (string Name, object Value)[] parameters) where T : INavigationWindow;

    Task DisplayWindow<T>() where T : INavigationWindow;

    void SetViewModel(NavigationViewModel viewModel);
}