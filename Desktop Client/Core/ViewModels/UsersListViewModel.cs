using Desktop_Client.Core.Abstracts;
using Desktop_Client.Core.Tools;
using Desktop_Client.Core.Tools.Attributes;
using Desktop_Client.Core.ViewModels.Base;
using Models.Client;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Desktop_Client.Core.ViewModels;

[Transient]
public sealed class UsersListViewModel : ViewModel
{
    private readonly IAPIClient _ApiClient;
    private readonly INavigationService _navigation;

    public UsersListViewModel(IAPIClient client, INavigationService navigation)
    {
        _ApiClient = client;
        _navigation = navigation;

        Task.Run(async () =>
        {
            Users = await _ApiClient.GetAsync<ObservableCollection<PublicUser>>("Users");
        });

        OpenUserProfile = new(async o =>
        {

        });
    }

    public ObservableCollection<PublicUser> Users { get; private set; }

    public PublicUser SelectedUser { get; set; }

    public UICommand OpenUserProfile { get; private init; }
}