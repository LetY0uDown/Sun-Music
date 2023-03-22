using Desktop_Client.Core.Abstracts;
using Desktop_Client.Core.Tools.Attributes;
using Desktop_Client.Core.ViewModels.Auth;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Desktop_Client.Views.Pages;

[HasLifetime(Lifetime.Transient)]
public partial class RegistrationPage : Page, INavigationPage
{
    private RegistrationViewModel _viewModel;

    public RegistrationPage (RegistrationViewModel viewModel)
    {
        _viewModel = viewModel;
    }

    public async Task Display ()
    {
        InitializeComponent();

        await _viewModel.Display();
        DataContext = _viewModel;
    }

    public async Task Leave()
    {
        await _viewModel.Leave();
    }

    private void passVisibileCB_Click (object sender, RoutedEventArgs e)
    {
        var isVisible = (bool)(sender as CheckBox).IsChecked;

        if (isVisible)
            passVisibilityIcon.Kind = Material.Icons.MaterialIconKind.Visibility;
        else
            passVisibilityIcon.Kind = Material.Icons.MaterialIconKind.VisibilityOff;
    }
}