using Desktop_Client.Core.Abstracts;
using Desktop_Client.Core.Tools.Attributes;
using Desktop_Client.Core.ViewModels.Tracks;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Desktop_Client.Views.Pages;

[HasLifetime(Lifetime.Transient)]
public partial class TracksPage : Page, INavigationPage
{
    private readonly TracksViewModel _viewModel;

    public TracksPage (TracksViewModel viewModel)
    {
        _viewModel = viewModel;
    }

    public async Task Display()
    {
        InitializeComponent();

        await _viewModel.Display();
        DataContext = _viewModel;
    }

    public async Task Leave()
    {
        await _viewModel.Leave();
    }
}