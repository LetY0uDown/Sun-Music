using Desktop_Client.Core.Abstracts;
using Desktop_Client.Core.Services;
using Desktop_Client.Core.Tools;
using Desktop_Client.Core.Tools.Attributes;
using Desktop_Client.Core.Tools.Extensions;
using Desktop_Client.Core.ViewModels.Base;
using Desktop_Client.Resources.UserControls;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Models.Database;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Desktop_Client.Core.ViewModels.Users;

[Lifetime(Lifetime.Transient)]
public sealed class UserProfileViewModel : ViewModel
{
    private readonly IAPIClient _apiClient;
    private readonly INavigationService _navigation;
    private readonly IHubFactory _hubFactory;
    private readonly IFileManager _fileManager;
    private readonly IMusicPlayer _musicPlayer;
    private readonly IConfiguration _config;

    private HubConnection _hub;

    public UserProfileViewModel (IAPIClient apiClient, INavigationService navigation, IHubFactory hubFactory, IFileManager fileManager, IMusicPlayer musicPlayer, IConfiguration config)
    {
        _apiClient = apiClient;
        _navigation = navigation;
        _hubFactory = hubFactory;
        _fileManager = fileManager;
        _musicPlayer = musicPlayer;
        _config = config;
    }

    public UICommand LikeTrackCommand { get; private set; }

    public UICommand ListenTrackCommand { get; private set; }

    public UICommand DownloadTrackCommand { get; private set; }

    public string UserID { get; set; }

    public User User { get; set; }

    public ObservableCollection<MusicTrack> Tracks { get; set; }

    public MusicTrack SelectedTrack { get; set; }

    public override async Task Display ()
    {
        User = await _apiClient.GetAsync<User>($"Users/{UserID}");
        var tracks = await _apiClient.GetAsync<IEnumerable<MusicTrack>>($"Likes/User/{UserID}");
        Tracks = new(tracks);

        _hub = await _hubFactory.CreateHub();
        await ConfigureHub();

        LikeTrackCommand = new(async o => {
            if (App.FaviriteTracksIDs.Contains(o.ToString())) {
                await _apiClient.PostAsync<object, object>(null, $"/Likes/Dislike/{o}/{App.AuthorizeData.ID}");
                App.FaviriteTracksIDs.Remove(o.ToString());
            } else {
                await _apiClient.PostAsync<object, object>(null, $"/Likes/Like/{o}/{App.AuthorizeData.ID}");
                App.FaviriteTracksIDs.Add(o.ToString());
            }
        });

        DownloadTrackCommand = new(async o => {
            var stream = await _fileManager.DownloadStream(SelectedTrack.ID, "Tracks/File");

            var path = Path.Combine(_config["DownloadedMusicPath"], SelectedTrack.FileName);

            using (FileStream fs = new(path, FileMode.OpenOrCreate)) {
                await stream.CopyToAsync(fs);
            }
        }, b => SelectedTrack is not null);

        ListenTrackCommand = new(o => {
            _musicPlayer.SetTrack(SelectedTrack);
            _musicPlayer.SetPlaylist(Tracks);
        }, b => SelectedTrack is not null);
    }

    public override async Task Leave ()
    {
        await _hub.LeaveGroup($"Likes-{UserID}");
    }

    private async Task ConfigureHub ()
    {
        await _hub.JoinGroup($"Likes-{UserID}");

        _hub.On<MusicTrack>("TrackLiked", track => {
            Tracks.Add(track);
        });
        
        _hub.On<MusicTrack>("TrackDisliked", track => {
            var findedTrack = Tracks.FirstOrDefault(t => t.ID == track.ID);
            Tracks.Remove(findedTrack);
        });
    }
}