using Desktop_Client.Core.Abstracts;
using Desktop_Client.Core.Tools;
using Desktop_Client.Core.Tools.Attributes;
using Desktop_Client.Core.ViewModels.Base;
using Desktop_Client.Views.Pages;
using System.Threading.Tasks;

namespace Desktop_Client.Core.ViewModels;

[Singleton]
public sealed class MainNavigationViewModel : NavigationViewModel
{
    private readonly INavigationService _navigation;

    public MainNavigationViewModel (INavigationService navigation)
    {
        _navigation = navigation;
    }

    public UICommand NavigateToTracksCommand { get; private set; }

    public UICommand NavigateToPlaylistsCommand { get; private set; }

    public UICommand NavigateToUsersCommand { get; private set; }

    public UICommand NavigateToChatsCommand { get; private set; }

    public UICommand NavigateToSettingsCommand { get; private set; }

    public override Task Initialize()
    {
        NavigateToUsersCommand = new(o => {
            _navigation.SetCurrentPage<UsersPage>();
        });

        NavigateToSettingsCommand = new(o => {
            _navigation.SetCurrentPage<OptionsPage>();
        });

        return base.Initialize();
    }
}