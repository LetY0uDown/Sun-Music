using Desktop_Client.Core.Abstracts;
using Desktop_Client.Core.Tools.Attributes;
using Desktop_Client.Core.ViewModels.Messanger;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Desktop_Client.Views.Pages;

[Lifetime(Lifetime.Transient)]
public partial class ChatsPage : Page, INavigationPage
{
    private readonly ChatsViewModel _viewModel;

    public ChatsPage(ChatsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
    }

    public async Task Display()
    {
        await _viewModel.Display();
    }

    public async Task Leave()
    {
        await _viewModel.Leave();
    }
}