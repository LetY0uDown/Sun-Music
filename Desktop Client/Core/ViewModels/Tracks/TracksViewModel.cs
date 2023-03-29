using Desktop_Client.Core.Abstracts;
using Desktop_Client.Core.Tools;
using Desktop_Client.Core.Tools.Attributes;
using Desktop_Client.Core.ViewModels.Base;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Models.Database;
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

    public UICommand ListenTrackCommand { get; private set; }

    public UICommand DownloadTrackCommand { get; private set; }

    public ObservableCollection<MusicTrack> Tracks { get; private set; }

    public MusicTrack SelectedTrack { get; set; }

    public string SearchText
    {
        get => _searchText;
        set {
            _searchText = value;
            Tracks = FindTracksByName(value);
        }
    }

    public override async Task Display()
    {
        DownloadTrackCommand = new(async o => {
            var stream = await _fileManager.DownloadStream(SelectedTrack.ID, "Tracks/File");

            var path = Path.Combine(_config["DownloadedMusicPath"], SelectedTrack.FileName);

            using (FileStream fs = new(path, FileMode.OpenOrCreate)) {
                await stream.CopyToAsync(fs);
            }

        }, b => SelectedTrack is not null);

        ListenTrackCommand = new(o => {
            _musicPlayer.SetTrack(SelectedTrack);
        }, b => SelectedTrack is not null);

        _hub = await _hubFactory.CreateHub();

        ConfigureHubActions();

        await _hub.SendAsync("JoinGroup", "Tracks");

        _tracks = await _apiClient.GetAsync<List<MusicTrack>>("Tracks");
        Tracks = new(_tracks);
    }

    public override async Task Leave()
    {
        await _hub.SendAsync("LeaveGroup", "Tracks");
    }

    private void ConfigureHubActions()
    {
        _hub.On<MusicTrack>("RecieveTrack", track => {
            _tracks.Add(track);

            Tracks = FindTracksByName(SearchText);
        });
    }

    private ObservableCollection<MusicTrack> FindTracksByName(string name)
    {
        return new(_tracks.Where(t =>
                        t.Title.ToLower()
                            .Contains(name.ToLower())));
    }
}