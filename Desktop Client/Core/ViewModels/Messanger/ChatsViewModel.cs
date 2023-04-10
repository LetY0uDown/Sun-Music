using Desktop_Client.Core.Abstracts;
using Desktop_Client.Core.Tools.Attributes;
using Desktop_Client.Core.ViewModels.Base;

namespace Desktop_Client.Core.ViewModels.Messanger;

[Lifetime(Lifetime.Transient)]
public sealed class ChatsViewModel : ViewModel
{
    private readonly IHubFactory _hubFactory;
    private readonly IAPIClient _client;

    public ChatsViewModel(IHubFactory hubFactory, IAPIClient client)
    {
        _hubFactory = hubFactory;
        _client = client;
    }
}