using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;

namespace Desktop_Client.Core.Abstracts;

public interface IHubFactory : IService
{
    Task<HubConnection> CreateHub(string route);
}