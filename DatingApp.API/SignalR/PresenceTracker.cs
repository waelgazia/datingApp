using System.Collections.Concurrent;

namespace DatingApp.API.SignalR;

/// <summary>
/// Track the users that are currently connected to Presence Hub. This class will hold
/// in memory the users and their connections to PresenceHub. This approach is not scalable
/// as this is only going to track users connected to Presence Hub running on this particular
/// server. If we have multiple server instance with a load balancer, this will track only
/// the users on one instance.
/// </summary>
public class PresenceTracker
{
    /// <summary>
    /// The user can be connected to the server via multiple connections (mobile,
    /// multiple tabs, different PC, .. etc) and the byte is just a placeholder
    /// ConcurrentDictionary<userId, ConcurrentDictionary<connectionId, byte>>
    /// </summary>
    private static readonly ConcurrentDictionary<string, ConcurrentDictionary<string, byte>> _onlineUsers = new();

    public Task UserConnected(string userId, string connectionId)
    {
        var userConnections = _onlineUsers.GetOrAdd(userId, _ => new ConcurrentDictionary<string, byte>());
        userConnections.TryAdd(connectionId, 0);

        return Task.CompletedTask;
    }

    public Task UserDisconnected(string userId, string connectionId)
    {
        if (_onlineUsers.TryGetValue(userId, out var userConnections))
        {
            userConnections.TryRemove(connectionId, out _);
            if (userConnections.IsEmpty)
            {
                _onlineUsers.TryRemove(userId, out _);
            }
        }

        return Task.CompletedTask;
    }

    public Task<string[]> GetOnlineUsers()
    {
        return Task.FromResult(_onlineUsers.Keys.OrderBy(k => k).ToArray());
    }

    public static Task<List<string>> GetUserConnections(string userId)
    {
        if (_onlineUsers.TryGetValue(userId, out var connections))
        {
            return Task.FromResult(connections.Keys.ToList());
        }

        return Task.FromResult(new List<string>());
    }
}
