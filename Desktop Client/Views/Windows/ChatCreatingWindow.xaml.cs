using Desktop_Client.Core.Abstracts;
using Desktop_Client.Core.Tools.Attributes;
using Desktop_Client.Core.ViewModels.Messanger;
using System.Threading.Tasks;
using System.Windows;

namespace Desktop_Client.Views.Windows;

[Lifetime(Lifetime.Transient)]
public partial class ChatCreatingWindow : Window, INavigationWindow
{
    private readonly ChatCreatingViewModel _viewModel;

    public ChatCreatingWindow(ChatCreatingViewModel viewModel)
    {
        _viewModel = viewModel;
    }

    public async Task Display()
    {
        InitializeComponent();

        await _viewModel.Display();
        DataContext = _viewModel;

        Show();
    }

    async Task INavigationWindow.Hide()
    {
        await _viewModel.Leave();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Hide();
    }

    private void TitleBar_LeftMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (Settings.Default.IsWindowDraggable)
            DragMove();
    }
}