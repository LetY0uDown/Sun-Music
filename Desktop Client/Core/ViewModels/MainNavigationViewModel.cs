using Desktop_Client.Core.Abstracts;
using Desktop_Client.Core.Tools;
using Desktop_Client.Core.Tools.Attributes;
using Desktop_Client.Core.ViewModels.Base;
using Desktop_Client.Views.Pages;

namespace Desktop_Client.Core.ViewModels;

[Singleton]
public sealed class MainNavigationViewModel : NavigationViewModel
{
    private readonly INavigationService _navigation;

    public MainNavigationViewModel(INavigationService navigation)
    {
        _navigation = navigation;

        NavigateToUsersCommand = new(o => {
            _navigation.SetCurrentPage<UsersPage>();
        });
    }

    public UICommand NavigateToTracksCommand { get; private init; }

    public UICommand NavigateToPlaylistsCommand { get; private init; }

    public UICommand NavigateToUsersCommand { get; private init; }

    public UICommand NavigateToChatsCommand { get; private init; }

    public UICommand NavigateToSettingsCommand { get; private init; }
}