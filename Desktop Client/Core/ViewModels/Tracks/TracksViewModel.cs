using Desktop_Client.Core.Abstracts;
using Desktop_Client.Core.Tools;
using Desktop_Client.Core.Tools.Attributes;
using Desktop_Client.Core.Tools.Extensions;
using Desktop_Client.Core.ViewModels.Base;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Models.Database;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Desktop_Client.Core.ViewModels.Tracks;

[Lifetime(Lifetime.Transient)]
public class TracksViewModel : ViewModel
{
    List<MusicTrack> _tracks;

    private bool _onlyFavorite;
    private string _searchText;

    private readonly IAPIClient _apiClient;
    private readonly IHubFactory _hubFactory;
    private readonly IConfiguration _config;
    private readonly IFileManager _fileManager;
    private readonly IMusicPlayer _musicPlayer;

    private HubConnection _hub;

    public TracksViewModel(IAPIClient apiClient, IHubFactory hubFactory, IConfiguration config, IFileManager fileManager, IMusicPlayer musicPlayer)
    {
        _apiClient = apiClient;
        _hubFactory = hubFactory;
        _config = config;
        _fileManager = fileManager;
        _musicPlayer = musicPlayer;
    }

    public event EventHandler FavoritesUpdated;

    public bool OnlyFavorite
    {
        get => _onlyFavorite;
        set
        {
            _onlyFavorite = value;
            Search();
        }
    }

    public UICommand LikeTrackCommand { get; private set; }

    public UICommand ListenTrackCommand { get; private set; }

    public UICommand DownloadTrackCommand { get; private set; }

    public ObservableCollection<MusicTrack> Tracks { get; private set; }

    public MusicTrack SelectedTrack { get; set; }

    public string SearchText
    {
        get => _searchText;
        set
        {
            _searchText = value;
            Search();
        }
    }

    public override async Task Display()
    {
        LikeTrackCommand = new(async o =>
        {
            if (App.FaviriteTracksIDs.Contains(o.ToString()))
            {
                await _apiClient.PostAsync<object, object>(null, $"/Likes/Dislike/{o}/{App.AuthorizeData.ID}");
                App.FaviriteTracksIDs.Remove(o.ToString());
            }
            else
            {
                await _apiClient.PostAsync<object, object>(null, $"/Likes/Like/{o}/{App.AuthorizeData.ID}");
                App.FaviriteTracksIDs.Add(o.ToString());
            }

            FavoritesUpdated?.Invoke(this, EventArgs.Empty);
        });

        DownloadTrackCommand = new(async o =>
        {
            var stream = await _fileManager.DownloadStream(SelectedTrack.ID, "Tracks/File");

            var path = Path.Combine(_config["DownloadedMusicPath"], SelectedTrack.FileName);

            using (FileStream fs = new(path, FileMode.OpenOrCreate)) {
                await stream.CopyToAsync(fs);
            }
        }, b => SelectedTrack is not null);

        ListenTrackCommand = new(o =>
        {
            _musicPlayer.SetTrack(SelectedTrack);
            _musicPlayer.SetPlaylist(Tracks);
        }, b => SelectedTrack is not null);

        _hub = await _hubFactory.CreateHub();

        await ConfigureHub();

        _tracks = await _apiClient.GetAsync<List<MusicTrack>>("Tracks");
        Tracks = new(_tracks);
    }

    public override async Task Leave()
    {
        await _hub.LeaveGroup("Tracks");
    }

    private async Task ConfigureHub()
    {
        await _hub.JoinGroup("Tracks");

        _hub.On<MusicTrack>("RecieveTrack", track =>
        {
            _tracks.Add(track);

            Search();
        });
    }

    private void Search()
    {
        IEnumerable<MusicTrack> finded = _tracks;

        if (SearchText is not null)
        {
            finded = finded.Where(t =>
                           t.Title.ToLower()
                                .Contains(SearchText.ToLower()));
        }

        if (OnlyFavorite)
        {
            finded = finded.Where(track => App.FaviriteTracksIDs.Contains(track.ID));
        }

        Tracks = new(finded);
    }
}