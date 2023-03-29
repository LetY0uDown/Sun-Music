﻿using Desktop_Client.Core.Abstracts;
using Desktop_Client.Core.Tools;
using Desktop_Client.Core.Tools.Attributes;
using Desktop_Client.Core.ViewModels.Base;
using Desktop_Client.Views.Pages;
using Desktop_Client.Views.Windows;
using Models.Client;
using Models.Database;
using System;
using System.Threading.Tasks;

namespace Desktop_Client.Core.ViewModels.Auth;

[Lifetime(Lifetime.Transient)]
public sealed class LoginViewModel : ViewModel
{
    private readonly IAPIClient _apiClient;
    private readonly INavigationService _navigation;

    public LoginViewModel(IAPIClient apiClient, INavigationService navigation)
    {
        _apiClient = apiClient;
        _navigation = navigation;
    }

    public UICommand LoginCommand { get; private set; }

    public UICommand RedirectToRegistrationCommand { get; private set; }

    public string Username { get; set; }

    public string Password { get; set; }

    public bool IsPasswordVisible { get; set; } = true;

    public override Task Display()
    {
        LoginCommand = new(async o => {
            User user = new() {
                ID = string.Empty,
                Username = Username,
                Password = Password,
                ImageBytes = Array.Empty<byte>()
            };

            var authData = await _apiClient.PostAsync<User, AuthorizeData>(user, "Login");

            if (authData is not null) {
                App.AuthorizeData = authData;
                await _navigation.SetMainWindow<MainWindow>();
            }
        });

        RedirectToRegistrationCommand = new(async o => {
            await _navigation.SetCurrentPage<RegistrationPage>();
        });

        return base.Display();
    }
}