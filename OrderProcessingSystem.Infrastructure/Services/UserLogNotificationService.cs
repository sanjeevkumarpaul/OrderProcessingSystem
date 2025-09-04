using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using OrderProcessingSystem.Contracts.Dto;
using OrderProcessingSystem.Infrastructure.Hubs;

namespace OrderProcessingSystem.Infrastructure.Services;

/// <summary>
/// Service for sending real-time notifications about user login events
/// Uses SignalR to broadcast notifications to connected clients
/// </summary>
public interface IUserLogNotificationService
{
    Task NotifyUserLoginAsync(LoginEventResponse loginEvent);
}

public class UserLogNotificationService : IUserLogNotificationService
{
    private readonly IHubContext<UserLogHub> _hubContext;
    private readonly ILogger<UserLogNotificationService> _logger;

    public UserLogNotificationService(IHubContext<UserLogHub> hubContext, ILogger<UserLogNotificationService> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    /// <summary>
    /// Broadcast a user login notification to all connected clients monitoring user activity
    /// </summary>
    /// <param name="loginEvent">The login event details to broadcast</param>
    public async Task NotifyUserLoginAsync(LoginEventResponse loginEvent)
    {
        try
        {
            _logger.LogInformation("🔔 Broadcasting user login notification: {UserName} as {EventType}", 
                loginEvent.UserName, loginEvent.Event);

            // Send to all clients in the UserActivityMonitors group
            // Send three separate parameters as expected by the C# SignalR client
            await _hubContext.Clients.Group("UserActivityMonitors")
                .SendAsync("UserLoggedIn", 
                    loginEvent.UserName, 
                    loginEvent.EventDate.ToString("yyyy-MM-dd HH:mm:ss"), 
                    "Unknown IP"); // You might want to add IP address to LoginEventResponse

            _logger.LogInformation("✅ Successfully broadcasted login notification for {UserName} to UserActivityMonitors group", loginEvent.UserName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error broadcasting user login notification for {UserName}", loginEvent.UserName);
        }
    }
}
