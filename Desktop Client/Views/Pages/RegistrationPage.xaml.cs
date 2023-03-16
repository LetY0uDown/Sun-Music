using Desktop_Client.Core.Abstracts;
using Desktop_Client.Core.Tools.Attributes;
using Desktop_Client.Core.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Desktop_Client.Views.Pages;

[Transient]
public partial class RegistrationPage : Page, INavigationPage
{
    private RegistrationViewModel _viewModel;

    public RegistrationPage (RegistrationViewModel viewModel)
    {
        _viewModel = viewModel;
    }

    public void Display ()
    {
        InitializeComponent();

        DataContext = _viewModel;
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