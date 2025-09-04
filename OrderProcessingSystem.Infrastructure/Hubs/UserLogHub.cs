using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace OrderProcessingSystem.Infrastructure.Hubs;

/// <summary>
/// SignalR Hub for real-time UserLog notifications
/// Handles broadcasting login events to connected clients
/// </summary>
public class UserLogHub : Hub
{
    private readonly ILogger<UserLogHub> _logger;

    public UserLogHub(ILogger<UserLogHub> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Called when a client connects to the hub
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "UserActivityMonitors");
        _logger.LogDebug("Client {ConnectionId} connected and added to UserActivityMonitors group", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Called when a client disconnects from the hub
    /// </summary>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogDebug("Client {ConnectionId} disconnected from UserLog Hub", Context.ConnectionId);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, "UserActivityMonitors");
        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Join the UserActivity monitoring group
    /// </summary>
    public async Task JoinUserActivityGroup()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "UserActivityMonitors");
        _logger.LogDebug("Client {ConnectionId} joined UserActivity monitoring group", Context.ConnectionId);
    }

    /// <summary>
    /// Join a specific group (for backward compatibility)
    /// </summary>
    public async Task JoinGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        _logger.LogDebug("Client {ConnectionId} joined group: {GroupName}", Context.ConnectionId, groupName);
    }

    /// <summary>
    /// Leave the UserActivity monitoring group
    /// </summary>
    public async Task LeaveUserActivityGroup()
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, "UserActivityMonitors");
        _logger.LogDebug("Client {ConnectionId} left UserActivity monitoring group", Context.ConnectionId);
    }
}
