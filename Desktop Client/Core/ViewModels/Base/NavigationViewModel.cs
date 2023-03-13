using Desktop_Client.Core.Abstracts;

namespace Desktop_Client.Core.ViewModels.Base;

public abstract class NavigationViewModel : ViewModel
{
    private INavigationPage _navigationPage;

    public INavigationPage CurrentPage
    {
        get => _navigationPage;
        set
        {
            _navigationPage = value;
            _navigationPage.Display();
        }
    }
}