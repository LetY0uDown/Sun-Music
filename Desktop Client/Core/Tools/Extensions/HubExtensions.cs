using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;

namespace Desktop_Client.Core.Tools.Extensions;

internal static class HubExtensions
{
    internal static async Task JoinGroup(this HubConnection hub, string groupName)
    {
        await hub.SendAsync("JoinGroup", groupName);
    }
    
    internal static async Task LeaveGroup(this HubConnection hub, string groupName)
    {
        await hub.SendAsync("LeaveGroup", groupName);
    }
}