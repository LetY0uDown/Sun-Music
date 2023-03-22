using Desktop_Client.Core.Abstracts;
using Desktop_Client.Core.Tools.Attributes;
using Desktop_Client.Core.ViewModels.Base;
using Microsoft.AspNetCore.SignalR.Client;
using Models.Database;
using System.Threading.Tasks;

namespace Desktop_Client.Core.ViewModels.Tracks;

[HasLifetime(Lifetime.Transient)]
public class TracksViewModel : ViewModel
{
    private readonly IAPIClient _apiClient;
    private readonly IHubFactory _hubFactory;

    private readonly HubConnection _hub;

    public TracksViewModel(IAPIClient apiClient, IHubFactory hubFactory)
    {
        _apiClient = apiClient;
        _hubFactory = hubFactory;
    }

    public override async Task Display()
    {
        await _hubFactory.CreateHub("Music");

        ConfigureHubActions();

        await _hub.SendAsync("JoinGroup", "Tracks");
    }

    public override async Task Leave()
    {
        await _hub.SendAsync("LeaveGroup", "Tracks");
    }

    private void ConfigureHubActions()
    {
        _hub.On<MusicTrack>("RecieveTrack", track => {

        });
    }
}