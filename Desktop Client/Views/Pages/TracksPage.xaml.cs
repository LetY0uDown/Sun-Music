using Desktop_Client.Core.Abstracts;
using Desktop_Client.Core.Tools.Attributes;
using Desktop_Client.Core.ViewModels.Tracks;
using Models.Database;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Desktop_Client.Views.Pages;

[Lifetime(Lifetime.Transient)]
public partial class TracksPage : Page, INavigationPage
{
    private readonly TracksViewModel _viewModel;
    private readonly IAPIClient _client;

    public TracksPage (TracksViewModel viewModel, IAPIClient client)
    {
        _viewModel = viewModel;
        _client = client;
    }
    public async Task Display ()
    {
        var favorite = await _client.GetAsync<IEnumerable<MusicTrack>>($"Tracks/User/{App.AuthorizeData.ID}");
        App.FaviriteTracksIDs = favorite.Select(track => track.ID).ToList();

        InitializeComponent();

        _viewModel.FavoritesUpdated += FavoritesUpdated;

        await _viewModel.Display();
        DataContext = _viewModel;
    }

    private void FavoritesUpdated (object sender, System.EventArgs e)
    {
        TracksList.Items.Refresh();
    }

    public async Task Leave ()
    {
        await _viewModel.Leave();
    }
}