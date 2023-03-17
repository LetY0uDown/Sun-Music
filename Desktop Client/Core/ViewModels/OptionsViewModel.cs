using Desktop_Client.Core.Abstracts;
using Desktop_Client.Core.Tools;
using Desktop_Client.Core.Tools.Attributes;
using Desktop_Client.Core.ViewModels.Base;
using Desktop_Client.Views.Windows;
using Models.Database;
using System.Threading.Tasks;

namespace Desktop_Client.Core.ViewModels;

[Transient]
public sealed class OptionsViewModel : ViewModel
{
    private readonly IAPIClient _apiClient;
    private readonly INavigationService _navigation;

    public OptionsViewModel(IAPIClient client, INavigationService navigation)
    {
        _apiClient = client;
        _navigation = navigation;
    }

    public UICommand UploadTrackCommand { get; private set; }

    public User CurrentUser { get; set; }

    public override async Task Initialize()
    {
        CurrentUser = await _apiClient.GetAsync<User>($"u/{App.AuthorizeData.ID}");

        UploadTrackCommand = new(async o => {
            await _navigation.DisplayWindow<TrackUploadingWindow>();
        });
    }
}