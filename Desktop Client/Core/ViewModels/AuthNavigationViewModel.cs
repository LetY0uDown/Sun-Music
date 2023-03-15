using Desktop_Client.Core.Abstracts;
using Desktop_Client.Core.Tools.Attributes;
using Desktop_Client.Core.ViewModels.Base;

namespace Desktop_Client.Core.ViewModels;

[Transient]
public sealed class AuthNavigationViewModel : NavigationViewModel
{
    private readonly INavigationService _navigation;

    public AuthNavigationViewModel(INavigationService navigation)
    {
        _navigation = navigation;
    }
}