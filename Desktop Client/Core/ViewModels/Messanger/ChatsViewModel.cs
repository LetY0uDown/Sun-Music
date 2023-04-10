using Desktop_Client.Core.Abstracts;
using Desktop_Client.Core.Tools;
using Desktop_Client.Core.Tools.Attributes;
using Desktop_Client.Core.Tools.Extensions;
using Desktop_Client.Core.ViewModels.Base;
using Microsoft.AspNetCore.SignalR.Client;
using Models.Database;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Desktop_Client.Core.ViewModels.Messanger;

[Lifetime(Lifetime.Transient)]
public sealed class ChatsViewModel : ViewModel
{
    private string _currentChatGroup;

    private readonly IHubFactory _hubFactory;
    private readonly IAPIClient _client;

    private HubConnection _hub;

    private Chat _selectedChat;

    public ChatsViewModel(IHubFactory hubFactory, IAPIClient client)
    {
        _hubFactory = hubFactory;
        _client = client;
    }

    public Chat SelectedChat
    {
        get => _selectedChat;
        set {
            _selectedChat = value;
        }
    }

    public UICommand SendMessageCommand { get; set; }

    public ObservableCollection<Message> Messages { get; set; }

    public ObservableCollection<Chat> Chats { get; set; }

    public override async Task Display()
    {
        _hub = await _hubFactory.CreateHub();
        await ConfigureHub();
    }

    public override async Task Leave()
    {
        await _hub.LeaveGroup("Chats");
        await _hub.LeaveGroup(_currentChatGroup);
    }

    private async Task ConfigureHub()
    {
        await _hub.JoinGroup("Chats");
    }
}