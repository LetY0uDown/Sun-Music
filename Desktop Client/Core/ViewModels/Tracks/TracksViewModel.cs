using Desktop_Client.Core.Abstracts;
using Desktop_Client.Core.Tools.Attributes;
using Desktop_Client.Core.ViewModels.Base;
using Microsoft.AspNetCore.SignalR.Client;
using Models.Database;
using System.Threading.Tasks;

namespace Desktop_Client.Core.ViewModels.Tracks;

[Singleton]
public class TracksViewModel : ViewModel
{
    private bool _isHubConfigured = false;

    private readonly IAPIClient _apiClient;
    private readonly HubConnection _hub;

    public TracksViewModel(IAPIClient apiClient, HubConnection hub)
    {
        _apiClient = apiClient;
        _hub = hub;
    }

    public override async Task Display()
    {
        await _hub.SendAsync("JoinGroup", "Tracks");

        if (!_isHubConfigured)
            ConfigureHubActions();
    }

    public override async Task Leave()
    {
        await _hub.SendAsync("LeaveGroup", "Tracks");
    }

    private void ConfigureHubActions()
    {
        _isHubConfigured = true;

        _hub.On<MusicTrack>("RecieveTrack", track => {

        });
    }
}