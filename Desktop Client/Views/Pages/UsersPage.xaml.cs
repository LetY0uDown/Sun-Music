using Desktop_Client.Core.Abstracts;
using Desktop_Client.Core.Tools.Attributes;
using Desktop_Client.Core.ViewModels;
using System.Windows.Controls;

namespace Desktop_Client.Views.Pages;

[Transient]
public partial class UsersPage : Page, INavigationPage
{
    private readonly UsersListViewModel _viewModel;

    public UsersPage (UsersListViewModel viewModel)
    {
        _viewModel = viewModel;
    }

    public void Display ()
    {
        InitializeComponent();

        DataContext = _viewModel;
    }
}