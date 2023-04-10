using Desktop_Client.Core.Abstracts;
using Desktop_Client.Core.Tools.Attributes;
using Desktop_Client.Core.ViewModels.Tracks;
using Models.Database;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Desktop_Client.Views.Pages;

[Lifetime(Lifetime.Transient), Parameter(typeof(string), nameof(PlaylistID))]
public partial class PlaylistCreatingPage : Page, INavigationPage
{
    private readonly PlaylistCreatingViewModel _viewModel;
    private readonly IAPIClient _client;

    public PlaylistCreatingPage (PlaylistCreatingViewModel viewModel, IAPIClient client)
    {
        _viewModel = viewModel;
        _client = client;
    }

    public string PlaylistID { get; set; }

    public async Task Display ()
    {
        InitializeComponent();

        if (!string.IsNullOrWhiteSpace(PlaylistID)) {
            TracksList.SelectionMode = SelectionMode.Single;

            SelectImageButton.Visibility = System.Windows.Visibility.Hidden;
            SavePlaylistButton.Visibility = System.Windows.Visibility.Hidden;

            titleTB.IsReadOnly = true;

            var tracks = await _client.GetAsync<IEnumerable<MusicTrack>>($"Playlists/Tracks/{PlaylistID}");
            _viewModel.PlaylistID = PlaylistID;
            _viewModel.Tracks = tracks.ToList();
        } else {
            TracksList.SelectionMode = SelectionMode.Multiple;
        }

        await _viewModel.Display();
        DataContext = _viewModel;
    }

    public async Task Leave ()
    {
        await _viewModel.Leave();
    }
}