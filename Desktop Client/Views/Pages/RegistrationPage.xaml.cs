using Desktop_Client.Core.Abstracts;
using Desktop_Client.Core.Tools.Attributes;
using Desktop_Client.Core.ViewModels.Auth;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Desktop_Client.Views.Pages;

[Lifetime(Lifetime.Transient)]
public partial class RegistrationPage : Page, INavigationPage
{
    private readonly RegistrationViewModel _viewModel;

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

    private void passTB_PasswordChanged (object sender, RoutedEventArgs e)
    {
        passwordPlaceholder.Visibility = string.IsNullOrWhiteSpace(passTB.Password)
                                            ? Visibility.Visible
                                            : Visibility.Hidden;
        _viewModel.Password = passTB.Password;
    }
}