using Desktop_Client.Core.Abstracts;
using Desktop_Client.Core.Tools.Attributes;
using Desktop_Client.Core.ViewModels;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Desktop_Client.Views.Pages;

[Transient]
public partial class OptionsPage : Page, INavigationPage
{
    private readonly OptionsViewModel _viewModel;

    public OptionsPage(OptionsViewModel viewModel)
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