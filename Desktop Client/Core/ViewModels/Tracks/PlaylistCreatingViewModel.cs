using Desktop_Client.Core.Abstracts;
using Desktop_Client.Core.Tools;
using Desktop_Client.Core.Tools.Attributes;
using Desktop_Client.Core.ViewModels.Base;
using Desktop_Client.Views.Pages;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Win32;
using Models.Database;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Desktop_Client.Core.ViewModels.Tracks;

[Lifetime(Lifetime.Transient)]
public sealed class PlaylistCreatingViewModel : ViewModel
{
    private Playlist _playlist;

    private readonly IAPIClient _client;
    private readonly IConfiguration _config;
    private readonly INavigationService _navigation;

    public PlaylistCreatingViewModel (IAPIClient client, IConfiguration config, INavigationService navigation)
    {
        _client = client;
        _config = config;
        _navigation = navigation;
        _playlist = new Playlist();
    }

    public string PlaylistID { get; set; }

    public UICommand CreatePlaylistCommand { get; private set; }

    public UICommand SelectImageCommand { get; private set; }

    public string Title { get; set; }

    public BitmapImage Image { get; set; }

    public List<MusicTrack> Tracks { get; set; }

    public override async Task Display ()
    {
        if (!string.IsNullOrWhiteSpace(PlaylistID)) {
            _playlist = await _client.GetAsync<Playlist>($"Playlists/{PlaylistID}");

            Title = _playlist.Title;
            Image = ImageConverter.ImageFromBytes(_playlist.ImageBytes);
        }

        Tracks ??= await _client.GetAsync<List<MusicTrack>>($"Likes/User/{App.AuthorizeData.ID}");

        SelectImageCommand = new(o => {
            var fileDialog = new OpenFileDialog {
                Filter = _config["Filters:Images"]
            };

            if (fileDialog.ShowDialog() == true) {
                Image = ImageConverter.CreateImageFromFile(fileDialog.FileName);

                _playlist.ImageBytes = ImageConverter.BytesFromImage(Image);
            }
        });

        CreatePlaylistCommand = new(async o => {
            IList items = (IList)o;
            var selectedTracks = items.Cast<MusicTrack>().ToList();

            if (selectedTracks is null || selectedTracks.Count == 0) {
                InfoBox.Show("Выберите треки для плейлиста!");
                return;
            }

            var user = await _client.GetAsync<User>($"Users/{App.AuthorizeData.ID}");
            _playlist.User = user;
            _playlist.UserID = user.ID;

            _playlist.Title = Title;
            _playlist.ImageBytes = ImageConverter.BytesFromImage(Image);
            _playlist.MusicTracks = selectedTracks;

            _playlist.ID = "id";

            await _client.PostAsync<Playlist, Playlist>(_playlist, "Playlists/Create");

            await _navigation.SetCurrentPage<OptionsPage>();
        }, b => !string.IsNullOrWhiteSpace(Title) &&
                Image is not null);
    }
}