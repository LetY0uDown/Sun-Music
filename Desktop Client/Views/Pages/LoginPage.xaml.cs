﻿using Desktop_Client.Core.Abstracts;
using Desktop_Client.Core.Tools.Attributes;
using Desktop_Client.Core.ViewModels;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Desktop_Client.Views.Pages;

[Transient]
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

        await _viewModel.Initialize();
        DataContext = _viewModel;
    }

    private void passVisibileCB_Click (object sender, System.Windows.RoutedEventArgs e)
    {

    }
}