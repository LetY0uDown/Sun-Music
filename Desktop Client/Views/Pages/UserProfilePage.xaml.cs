﻿using Desktop_Client.Core.Abstracts;
using Desktop_Client.Core.Tools.Attributes;
using Desktop_Client.Core.ViewModels.Users;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Desktop_Client.Views.Pages;

[Lifetime(Lifetime.Transient), Parameter(typeof(string), nameof(UserID))]
public partial class UserProfilePage : Page, INavigationPage
{
    private readonly UserProfileViewModel _viewModel;

    public UserProfilePage (UserProfileViewModel viewModel)
    {
        _viewModel = viewModel;
    }

    public string UserID { get; set; }

    public async Task Display ()
    {
        _viewModel.UserID = UserID;

        await _viewModel.Display();
        DataContext = _viewModel;

        InitializeComponent();
    }

    public async Task Leave()
    {
        await _viewModel.Leave();
    }
}