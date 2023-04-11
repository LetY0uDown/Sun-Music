using Desktop_Client.Core.Abstracts;
using Desktop_Client.Core.Tools;
using Desktop_Client.Core.Tools.Attributes;
using Desktop_Client.Core.Tools.Extensions;
using Desktop_Client.Core.ViewModels.Base;
using Desktop_Client.Views.Pages;
using Desktop_Client.Views.Windows;
using Microsoft.AspNetCore.SignalR.Client;
using Models.Client;
using Models.Database;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Desktop_Client.Core.ViewModels.Messanger;

[Lifetime(Lifetime.Transient)]
public sealed class ChatsViewModel : ViewModel
{
    private List<Chat> _allChats;
    private IEnumerable<Chat> _myChats;

    private readonly IHubFactory _hubFactory;
    private readonly IAPIClient _client;
    private readonly INavigationService _navigation;
    private HubConnection _hub;
    
    private string _searchText = string.Empty;

    public ChatsViewModel(IHubFactory hubFactory, IAPIClient client, INavigationService navigation)
    {
        _hubFactory = hubFactory;
        _client = client;
        _navigation = navigation;
    }

    public ChatPage ChatPage { get; set; }

    public Chat SelectedChat { get; set; }

    public string SearchText
    {
        get => _searchText;
        set {
            _searchText = value;
            Search();
        }
    }

    public UICommand JoinChatCommand { get; private set; }

    public UICommand CreateChatCommand { get; private set; }

    public ObservableCollection<Chat> Chats { get; private set; }

    public override async Task Display()
    {
        JoinChatCommand = new(async o => {
            await ChatPage?.Leave();

            var page = (ChatPage)App.Host.Services.GetService(typeof(ChatPage));
            page.ChatID = SelectedChat.ID;

            ChatPage = page;
            await page.Display();

        }, b => SelectedChat is not null);

        CreateChatCommand = new(async o => {
            await _navigation.DisplayWindow<ChatCreatingWindow>();
        });

        _hub = await _hubFactory.CreateHub();
        await ConfigureHub();

        _allChats = await _client.GetAsync<List<Chat>>("Chats");

        _myChats = await _client.GetAsync<IEnumerable<Chat>>($"Chats/User/{App.AuthorizeData.ID}");
        Chats = new ObservableCollection<Chat>(_myChats);
    }

    public override async Task Leave()
    {
        await _hub.LeaveGroup("Chats");
    }

    private async Task ConfigureHub()
    {
        await _hub.JoinGroup("Chats");

        _hub.On<Chat>("RecieveChat", chat => {
            _allChats.Add(chat);
            Search();
        });
    }

    private void Search()
    {
        if (string.IsNullOrWhiteSpace(SearchText)) {
            Chats = new(_myChats);
        } else {
            Chats = new(_allChats.Where(chat => chat.Title.ToLower()
                                                .Contains(SearchText.ToLower())));
        }
    }
}