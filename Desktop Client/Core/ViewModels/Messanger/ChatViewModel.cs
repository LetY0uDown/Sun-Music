using Desktop_Client.Core.Abstracts;
using Desktop_Client.Core.Tools;
using Desktop_Client.Core.Tools.Attributes;
using Desktop_Client.Core.Tools.Extensions;
using Desktop_Client.Core.ViewModels.Base;
using Microsoft.AspNetCore.SignalR.Client;
using Models.Client;
using Models.Database;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Threading.Tasks;

namespace Desktop_Client.Core.ViewModels.Messanger;

[Lifetime(Lifetime.Transient)]
public sealed class ChatViewModel : ViewModel
{
    private readonly IHubFactory _hubFactory;
    private readonly IAPIClient _client;

    private string _currentChatGroup;

    private HubConnection _hub;

    public ChatViewModel(IHubFactory hubFactory, IAPIClient client)
    {
        _hubFactory = hubFactory;
        _client = client;
    }

    public string MessageText { get; set; }

    public Chat Chat { get; set; }

    public UICommand SendMessageCommand { get; private set; }

    public ObservableCollection<Message> Messages { get; private set; }

    public ObservableCollection<PublicUser> CurrentChatMembers { get; private set; }

    public override async Task Display()
    {
        await ConfigureHub();

        Messages = new(await _client.GetAsync<IEnumerable<Message>>($"Chats/{Chat.ID}/Messages"));
        CurrentChatMembers = new(await _client.GetAsync<IEnumerable<PublicUser>>($"Chats/{Chat.ID}/Members"));

        SendMessageCommand = new(async o => {
            Message msg = new() {
                ID = System.Guid.Empty,
                ChatID = Chat.ID,
                Chat = Chat,
                SenderID = App.AuthorizeData.ID,
                Sender = await _client.GetAsync<User>($"Users/{App.AuthorizeData.ID}"),
                Text = MessageText,
                TimeSended = System.DateTime.Now
            };

            await _client.PostAsync<Message, object>(msg, $"Chats/{Chat.ID}/Send/{App.AuthorizeData.ID}");

            MessageText = string.Empty;

        }, b => !string.IsNullOrWhiteSpace(MessageText));
    }

    public override async Task Leave()
    {
        await _hub.LeaveGroup(_currentChatGroup);
    }

    private async Task ConfigureHub()
    {
        _hub = await _hubFactory.CreateHub();

        _currentChatGroup = $"Chat - {Chat.Title}";
        await _hub.JoinGroup(_currentChatGroup);

        _hub.On<PublicUser>("RecieveMember", member => {
            CurrentChatMembers.Add(member);
        });
        
        _hub.On<PublicUser>("RemoveMember", member => {
            CurrentChatMembers.Remove(member);
        });

        _hub.On<Message>("RecieveMessage", msg => {
            Messages.Add(msg);
        });
    }
}