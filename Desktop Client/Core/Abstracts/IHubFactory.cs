using Desktop_Client.Core.Tools.Attributes;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;

namespace Desktop_Client.Core.Abstracts;

[BaseType]
public interface IHubFactory : IService
{
    Task<HubConnection> CreateHub();
}