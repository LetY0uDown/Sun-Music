using Desktop_Client.Core.Tools.Attributes;
using Desktop_Client.Core.ViewModels;
using System.Windows.Controls;

namespace Desktop_Client.Views.Pages;

[Transient]
public partial class LoginPage : Page
{
    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();

        DataContext = viewModel;
    }

    private void passVisibileCB_Click (object sender, System.Windows.RoutedEventArgs e)
    {

    }
}