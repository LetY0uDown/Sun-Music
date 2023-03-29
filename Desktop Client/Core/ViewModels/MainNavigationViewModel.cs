using Desktop_Client.Core.Abstracts;
using Desktop_Client.Core.Tools;
using Desktop_Client.Core.Tools.Attributes;
using Desktop_Client.Core.ViewModels.Base;
using Desktop_Client.Resources.UserControls;
using Desktop_Client.Views.Pages;
using System.Threading.Tasks;

namespace Desktop_Client.Core.ViewModels;

[Lifetime(Lifetime.Transient)]
public sealed class MainNavigationViewModel : NavigationViewModel
{
    private readonly INavigationService _navigation;
    private readonly IMusicPlayer _musicPlayer;

    public MainNavigationViewModel(INavigationService navigation, IMusicPlayer musicPlayer)
    {
        _navigation = navigation;
        _musicPlayer = musicPlayer;
    }

    public IMusicPlayer MusicPlayer { get; private set; }

    public UICommand NavigateToTracksCommand { get; private set; }

    public UICommand NavigateToPlaylistsCommand { get; private set; }

    public UICommand NavigateToUsersCommand { get; private set; }

    public UICommand NavigateToChatsCommand { get; private set; }

    public UICommand NavigateToSettingsCommand { get; private set; }

    public override async Task Display()
    {
        MusicPlayer = _musicPlayer;

        NavigateToUsersCommand = new(async o => {
            await _navigation.SetCurrentPage<UsersPage>();
        });

        NavigateToSettingsCommand = new(async o => {
            await _navigation.SetCurrentPage<OptionsPage>();
        });

        NavigateToTracksCommand = new(async o => {
            await _navigation.SetCurrentPage<TracksPage>();
        });

        await _navigation.SetCurrentPage<TracksPage>();
    }
}