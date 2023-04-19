using Desktop_Client.Core.Abstracts;
using Desktop_Client.Core.Tools.Attributes;
using Desktop_Client.Core.ViewModels.Auth;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Desktop_Client.Views.Pages;

[Lifetime(Lifetime.Transient)]
public partial class LoginPage : Page, INavigationPage
{
    private readonly LoginViewModel _viewModel;

    public LoginPage (LoginViewModel viewModel)
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

    private void passTB_PasswordChanged (object sender, System.Windows.RoutedEventArgs e)
    {
        passwordPlaceholder.Visibility = string.IsNullOrWhiteSpace(passTB.Password) 
                                            ? System.Windows.Visibility.Visible
                                            : System.Windows.Visibility.Hidden;
        _viewModel.Password = passTB.Password;
    }
}