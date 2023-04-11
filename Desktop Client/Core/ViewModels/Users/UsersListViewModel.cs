using Desktop_Client.Core.Abstracts;
using Desktop_Client.Core.Tools;
using Desktop_Client.Core.Tools.Attributes;
using Desktop_Client.Core.Tools.Extensions;
using Desktop_Client.Core.ViewModels.Base;
using Desktop_Client.Views.Pages;
using Microsoft.AspNetCore.SignalR.Client;
using Models.Client;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Desktop_Client.Core.ViewModels.Users;

[Lifetime(Lifetime.Transient)]
public sealed class UsersListViewModel : ViewModel
{
    private string _searchText;

    private IEnumerable<PublicUser> _usersOriginal;

    private readonly IAPIClient _apiClient;
    private readonly INavigationService _navigation;
    private readonly IHubFactory _hubFactory;

    private HubConnection _hub;

    public UsersListViewModel (IAPIClient client, INavigationService navigation, IHubFactory hubFactory)
    {
        _apiClient = client;
        _navigation = navigation;
        _hubFactory = hubFactory;
    }

    public string SearchText
    {
        get => _searchText;

        set {
            _searchText = value;
            Users = new(_usersOriginal.Where(u => 
                                           u.Username.ToLower()
                                                .Contains(SearchText.ToLower())));
        }
    }

    public ObservableCollection<PublicUser> Users { get; private set; }

    public PublicUser SelectedUser { get; set; }

    public UICommand OpenUserProfile { get; private set; }

    public override async Task Display()
    {
        _hub = await _hubFactory.CreateHub();
        await ConfigureHub();

        _usersOriginal = await _apiClient.GetAsync<ObservableCollection<PublicUser>>("Users");
        Users = new(_usersOriginal);

        OpenUserProfile = new(o => {
            _navigation.SetCurrentPage<UserProfilePage>(("UserID", SelectedUser.ID));
        }, b => SelectedUser is not null);
    }

    private async Task ConfigureHub()
    {
        await _hub.JoinGroup("Users");

        _hub.On<PublicUser>("RecieveUser", user => {
            Users.Add(user);

            Users = new(_usersOriginal.Where(u =>
                                           u.Username.ToLower()
                                                .Contains(SearchText?.ToLower())));
        });
    }
}