using Desktop_Client.Core.Abstracts;
using Desktop_Client.Core.Tools;
using Desktop_Client.Core.Tools.Attributes;
using Desktop_Client.Core.ViewModels.Base;
using Desktop_Client.Properties;
using Desktop_Client.Views.Pages;
using Desktop_Client.Views.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Win32;
using Models.Database;
using System.Threading.Tasks;

namespace Desktop_Client.Core.ViewModels;

[Lifetime(Lifetime.Transient)]
public sealed class OptionsViewModel : ViewModel
{
    private readonly IAPIClient _apiClient;
    private readonly INavigationService _navigation;
    private readonly IConfiguration _config;

    public OptionsViewModel (IAPIClient client, INavigationService navigation, IConfiguration config)
    {
        _apiClient = client;
        _navigation = navigation;
        _config = config;
    }

    public string MusicSavePath
    {
        get => _config["DownloadedMusicPath"];
        set => _config["DownloadedMusicPath"] = value;
    }

    public bool SaveAuthData
    {
        get => Settings.Default.DoRememberData;
        set {
            Settings.Default.DoRememberData = value;
            Settings.Default.Save();
        }
    }
    
    public bool IsWindowDraggable
    {
        get => Settings.Default.IsWindowDraggable;
        set {
            Settings.Default.IsWindowDraggable = value;
            Settings.Default.Save();
        }
    }

    public UICommand SelectImageCommand { get; private set; }

    public UICommand SaveProfileCommand { get; private set; }

    public UICommand UploadTrackCommand { get; private set; }

    public UICommand CreatePlaylistCommand { get; private set; }

    public User CurrentUser { get; set; }

    public override async Task Display()
    {
        CurrentUser = await _apiClient.GetAsync<User>($"Users/{App.AuthorizeData.ID}");

        SelectImageCommand = new(o => {
            var fileDialog = new OpenFileDialog {
                Filter = _config["Filters:Images"]
            };

            if (fileDialog.ShowDialog() == true) {
                var image = ImageConverter.CreateImageFromFile(fileDialog.FileName);
                CurrentUser.ImageBytes = ImageConverter.BytesFromImage(image);
            }
        });

        SaveProfileCommand = new(async o => {
            await _apiClient.PutAsync<User, object>(CurrentUser, $"Users/Edit/{CurrentUser.ID}");
        }, b => !string.IsNullOrWhiteSpace(CurrentUser.Username) &&
                CurrentUser.ImageBytes is not null);

        UploadTrackCommand = new(async o => {
            await _navigation.DisplayWindow<TrackUploadingWindow>();
        });

        CreatePlaylistCommand = new(async o => {
            await _navigation.SetCurrentPage<PlaylistCreatingPage>();
        });
    }
}