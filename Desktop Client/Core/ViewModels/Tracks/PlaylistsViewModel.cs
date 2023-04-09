using Desktop_Client.Core.Abstracts;
using Desktop_Client.Core.Tools.Attributes;
using Desktop_Client.Core.ViewModels.Base;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Models.Client;
using Models.Database;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Desktop_Client.Core.ViewModels.Tracks;

[Lifetime(Lifetime.Transient)]
public sealed class PlaylistsViewModel : ViewModel
{
    private ObservableCollection<Playlist> _playlistOrigin;

    private readonly IAPIClient _client;
    private readonly IHubFactory _hubFactory;
    private readonly IConfiguration _config;
    private readonly IMusicPlayer _musicPlayer;

    private HubConnection _hub;

    private string _searchText = string.Empty;
    private PublicUser _selectedUser;

    public PlaylistsViewModel (IAPIClient client, IHubFactory hubFactory, IConfiguration config, IMusicPlayer musicPlayer)
    {
        _client = client;
        _hubFactory = hubFactory;
        _config = config;
        _musicPlayer = musicPlayer;
    }

    public ObservableCollection<Playlist> Playlists { get; set; }

    public Playlist SelectedPlaylist { get; set; }

    public List<PublicUser> Users { get; set; }

    public PublicUser SelectedUser
    {
        get => _selectedUser;
        set {
            _selectedUser = value;
            Search();
        }
    }

    public string SearchText
    {
        get => _searchText;
        set {
            _searchText = value;

            if (!string.IsNullOrWhiteSpace(value)) {
                Search();
            }
        }
    }

    public override async Task Display ()
    {
        var playlists = await _client.GetAsync<IEnumerable<Playlist>>("Playlists");

        _playlistOrigin = new(playlists);
        Playlists = _playlistOrigin;

        Users = await _client.GetAsync<List<PublicUser>>("Users");
        Users.Insert(0, new() {
            ID = Guid.Empty.ToString(),
            Username = "- - -"
        });

        SelectedUser = Users[0];

        _hub = await _hubFactory.CreateHub();
        ConfigureHubActions();
        await _hub.SendAsync("JoinGroup", "Playlists");
    }

    public override async Task Leave ()
    {
        await _hub.SendAsync("LeaveGroup", "Playlists");
    }

    private void ConfigureHubActions ()
    {
        _hub.On<Playlist>("RecievePlaylist", playlist => {
            _playlistOrigin.Add(playlist);
            Playlists.Add(playlist);
        });
    }

    private void Search ()
    {
        var finded = _playlistOrigin.Where(playlist => playlist.Title.ToLower().Contains(SearchText.ToLower()));

        if (SelectedUser is not null && SelectedUser.Username != "- - -") {
            finded = finded.Where(playlist => playlist.UserID == SelectedUser.ID);
        }

        Playlists = new(finded);
    }
}