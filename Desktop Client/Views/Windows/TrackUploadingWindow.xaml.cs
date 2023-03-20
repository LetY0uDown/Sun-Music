using Desktop_Client.Core.Abstracts;
using Desktop_Client.Core.Tools.Attributes;
using Desktop_Client.Core.ViewModels;
using System.Threading.Tasks;
using System.Windows;

namespace Desktop_Client.Views.Windows;

[Transient]
public partial class TrackUploadingWindow : Window, INavigationWindow
{
    private readonly TrackUploadingViewModel _viewModel;

    public TrackUploadingWindow(TrackUploadingViewModel viewModel)
    {
        _viewModel = viewModel;
    }

    public async Task Display()
    {
        InitializeComponent();

        await _viewModel.Initialize();

        DataContext = _viewModel;

        Show();
    }

    Task INavigationWindow.Hide()
    {
        Hide();

        return Task.CompletedTask;
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void TitleBar_LeftMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        DragMove();
    }
}