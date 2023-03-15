﻿using Desktop_Client.Core.Abstracts;
using Desktop_Client.Core.Tools.Attributes;
using Desktop_Client.Core.ViewModels.Base;
using Models.Database;
using System.Threading.Tasks;

namespace Desktop_Client.Core.ViewModels;

[Transient]
public sealed class UserProfileViewModel : ViewModel
{
    private readonly IAPIClient _apiClient;
    private readonly INavigationService _navigation;

    public UserProfileViewModel(IAPIClient apiClient, INavigationService navigation)
    {
        _apiClient = apiClient;
        _navigation = navigation;
    }

    public string UserID
    {
        set
        {
            Task.Run(async () => User = await _apiClient.GetAsync<User>($"u/{value}"));
        }
    }

    public User User { get; set; }
}