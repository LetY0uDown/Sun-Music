using Desktop_Client.Core.Abstracts;
using Desktop_Client.Core.Tools.Attributes;
using Desktop_Client.Core.ViewModels.Messanger;
using Models.Database;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Desktop_Client.Views.Pages;

[Lifetime(Lifetime.Transient)]
public partial class ChatPage : Page, INavigationPage
{
    private readonly ChatViewModel _viewModel;
    private readonly IAPIClient _client;

    public ChatPage(ChatViewModel viewModel, IAPIClient client)
    {
        _viewModel = viewModel;
        _client = client;
    }

    public string ChatID { get; set; }

    public async Task Display()
    {
        InitializeComponent();

        var chat = await _client.GetAsync<Chat>($"Chats/{ChatID}");
        _viewModel.Chat = chat;

        await _viewModel.Display();
        DataContext = _viewModel;
    }

    public async Task Leave()
    {
        await _viewModel.Leave();
    }
}