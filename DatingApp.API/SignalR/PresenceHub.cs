using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;

using DatingApp.API.Extensions;

namespace DatingApp.API.SignalR;

[Authorize]
public class PresenceHub(PresenceTracker _presenceTracker) : Hub
{
    public override async Task OnConnectedAsync()
    {
        await _presenceTracker.UserConnected(GetUserId(), Context.ConnectionId);

        // UserOnline is the name of the method that the client is going
        // to listen for to receive notification when the user is online.
        await Clients.Others.SendAsync("UserOnline", GetUserId());

        string[] onlineUsers = await _presenceTracker.GetOnlineUsers();
        await Clients.Caller.SendAsync("GetOnlineUsers", onlineUsers);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await _presenceTracker.UserDisconnected(GetUserId(), Context.ConnectionId);

        await Clients.Others.SendAsync("UserOffline", GetUserId());

        await base.OnDisconnectedAsync(exception);
    }

    private string GetUserId()
    {
        return Context.User?.GetMemberId() ?? throw new HubException("Cannot get member id");
    }
}
