namespace API.Hubs;

public class PresenceTracker
{
    private static readonly Dictionary<string, List<string>> OnlineUsers = [];

    public Task UserConnectedAsync(string username, string connectionId)
    {
        lock (OnlineUsers)
        {
            if (OnlineUsers.ContainsKey(username))
            {
                OnlineUsers[username].Add(connectionId);
            }
            else
            {
                OnlineUsers.Add(username, [connectionId]);
            }
        }

        return Task.CompletedTask;
    }

    public Task UserDisconnectedAsync(string username, string connectionId)
    {
        lock (OnlineUsers)
        {
            if (OnlineUsers.ContainsKey(username))
            {
                OnlineUsers[username].Remove(connectionId);
                if (OnlineUsers[username].Count == 0)
                {
                    OnlineUsers.Remove(username);
                }
            }
        }

        return Task.CompletedTask;
    }

    public Task<string[]> GetOnlineUsersAsync()
    {
        string[] onlineUsers;
        lock (OnlineUsers)
        {
            onlineUsers = OnlineUsers.Keys.Order().ToArray();
        }

        return Task.FromResult(onlineUsers);
    }
}
