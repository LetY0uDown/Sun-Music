using Desktop_Client.Core.Tools.Attributes;
using Desktop_Client.Core.ViewModels;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Desktop_Client.Views.Pages;

[Transient]
public partial class RegistrationPage : Page
{
    private RegistrationViewModel _viewModel;

    public RegistrationPage(RegistrationViewModel viewModel)
    {
        InitializeComponent();
        
        _viewModel = viewModel;
        DataContext = _viewModel;
    }

    private void passVisibileCB_Click(object sender, RoutedEventArgs e)
    {
        var isVisible = (bool)(sender as CheckBox).IsChecked;

        if (isVisible)
            passVisibilityIcon.Kind = Material.Icons.MaterialIconKind.Visibility;
        else
            passVisibilityIcon.Kind = Material.Icons.MaterialIconKind.VisibilityOff;
    }
}