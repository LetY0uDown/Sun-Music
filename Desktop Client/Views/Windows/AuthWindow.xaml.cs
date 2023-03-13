using Desktop_Client.Core.Abstracts;
using Desktop_Client.Core.Tools.Attributes;
using Desktop_Client.Core.ViewModels;
using Desktop_Client.Views.Pages;
using System.Windows;

namespace Desktop_Client.Views.Windows;

[Transient]
public partial class AuthWindow : Window
{
    public AuthWindow(AuthNavigationViewModel viewModel, INavigationService navigation)
    {
        InitializeComponent();

        DataContext = viewModel;
        navigation.SetViewModel(viewModel);

        navigation.SetCurrentPage<RegistrationPage>();
    }

    private void MinimizeButton_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void Titlebar_LeftMouseButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        DragMove();
    }
}