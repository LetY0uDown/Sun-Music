using Desktop_Client.Core.Abstracts;
using Desktop_Client.Core.Tools.Attributes;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Desktop_Client.Core.Services;

[Lifetime(Lifetime.Transient)]
internal sealed class HubFactory : IHubFactory
{
    private readonly IConfiguration _config;

    public HubFactory(IConfiguration config)
    {
        _config = config;
    }

    public async Task<HubConnection> CreateHub()
    {
        var connection = new HubConnectionBuilder()
                            .WithUrl(_config["HostURL:HTTP"] + "MainHub")
                            .WithAutomaticReconnect()
                            .Build();

        await connection.StartAsync();

        return connection;
    }
}