using API.Extentions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.Hubs;

[Authorize]
public class PresenceHub(PresenceTracker tracker) : Hub
{
    public override async Task OnConnectedAsync()
    {
        if (Context.User == null)
        {
            throw new HubException("Cannot get current user claim");
        }

        await tracker.UserConnectedAsync(Context.User.GetUsername(), Context.ConnectionId);
        await Clients.Others.SendAsync("UserIsOnline", Context.User?.GetUsername());

        var currentUsers = await tracker.GetOnlineUsersAsync();
        await Clients.All.SendAsync("GetOnlineUsers", currentUsers);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (Context.User == null)
        {
            throw new HubException("Cannot get current user claim");
        }

        await tracker.UserDisconnectedAsync(Context.User.GetUsername(), Context.ConnectionId);
        await Clients.Others.SendAsync("UserIdOffline", Context.User?.GetUsername());

        var currentUsers = await tracker.GetOnlineUsersAsync();
        await Clients.All.SendAsync("GetOnlineUsers", currentUsers);

        await base.OnDisconnectedAsync(exception);
    }
}