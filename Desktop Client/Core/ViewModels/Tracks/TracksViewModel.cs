using Desktop_Client.Core.Abstracts;
using Desktop_Client.Core.Tools;
using Desktop_Client.Core.Tools.Attributes;
using Desktop_Client.Core.ViewModels.Base;
using Microsoft.AspNetCore.SignalR.Client;
using Models.Database;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

    private HubConnection _hub;

    public TracksViewModel(IAPIClient apiClient, IHubFactory hubFactory)
    {
        _apiClient = apiClient;
        _hubFactory = hubFactory;
    }

    public UICommand DownloadTrackCommand { get; private set; }

    public ObservableCollection<MusicTrack> Tracks { get; private set; }

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

        });

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