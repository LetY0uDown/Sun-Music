using Desktop_Client.Core.Abstracts;
using Desktop_Client.Core.Tools.Attributes;
using Desktop_Client.Core.ViewModels;
using System.Windows.Controls;

namespace Desktop_Client.Views.Pages;

[Transient, Parameter(typeof(string), nameof(UserID))]
public partial class UserProfilePage : Page, INavigationPage
{
    private readonly UserProfileViewModel _viewModel;

    public UserProfilePage(UserProfileViewModel viewModel)
    {
        _viewModel = viewModel;
    }

    public string UserID { get; set; }

    public void Display()
    {
        InitializeComponent();

        _viewModel.UserID = UserID;
        DataContext = _viewModel;
    }
}