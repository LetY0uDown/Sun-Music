using Desktop_Client.Core.Abstracts;
using Desktop_Client.Core.Tools;
using Desktop_Client.Core.Tools.Attributes;
using Desktop_Client.Core.ViewModels.Base;
using Desktop_Client.Views.Pages;
using Models.Client;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace Desktop_Client.Core.ViewModels;

[Transient]
public sealed class UsersListViewModel : ViewModel
{
    private string _searchText;

    private IEnumerable<PublicUser> _usersOriginal;

    private readonly IAPIClient _ApiClient;
    private readonly INavigationService _navigation;

    public UsersListViewModel(IAPIClient client, INavigationService navigation)
    {
        _ApiClient = client;
        _navigation = navigation;

        Task.Run(async () => {
            _usersOriginal = await _ApiClient.GetAsync<ObservableCollection<PublicUser>>("Users");
            Users = new(_usersOriginal);
        });

        OpenUserProfile = new(async o => {
            _navigation.SetCurrentPage<UserProfilePage>(("UserID", SelectedUser.ID));
        }, b => SelectedUser is not null);
    }

    public string SearchText
    {
        get => _searchText;

        set
        {
            _searchText = value;
            Users = new(_usersOriginal.Where(u => u.Username.ToLower().Contains(SearchText.ToLower())));
        }
    }

    public ObservableCollection<PublicUser> Users { get; private set; }

    public PublicUser SelectedUser { get; set; }

    public UICommand OpenUserProfile { get; private init; }
}