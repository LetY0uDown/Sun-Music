using Desktop_Client.Core.Abstracts;

namespace Desktop_Client.Core.ViewModels.Base;

public abstract class NavigationViewModel : ViewModel
{
    public INavigationPage CurrentPage { get; set; }
}