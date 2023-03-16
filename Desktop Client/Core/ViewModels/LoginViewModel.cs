﻿using Desktop_Client.Core.Abstracts;
using Desktop_Client.Core.Tools;
using Desktop_Client.Core.Tools.Attributes;
using Desktop_Client.Core.ViewModels.Base;
using Desktop_Client.Views.Pages;
using Desktop_Client.Views.Windows;
using Models.Client;
using Models.Database;
using System;

namespace Desktop_Client.Core.ViewModels;

[Transient]
public sealed class LoginViewModel : ViewModel
{
    private readonly IAPIClient _apiClient;
    private readonly INavigationService _navigation;

    public LoginViewModel (IAPIClient apiClient, INavigationService navigation)
    {
        _apiClient = apiClient;
        _navigation = navigation;

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
                _navigation.SetMainWindow<MainWindow>();
            }
        });

        RedirectToRegistrationCommand = new(o => {
            _navigation.SetCurrentPage<RegistrationPage>();
        });
    }

    public UICommand LoginCommand { get; private init; }

    public UICommand RedirectToRegistrationCommand { get; private init; }

    public string Username { get; set; }

    public string Password { get; set; }

    public bool IsPasswordVisible { get; set; } = true;
}