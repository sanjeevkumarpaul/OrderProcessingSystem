using Microsoft.AspNetCore.Mvc;
using OrderProcessingSystem.Infrastructure.Services;
using OrderProcessingSystem.Contracts.Dto;

namespace OrderProcessingSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserLogController : ControllerBase
{
    private readonly UserLogService _userLogService;
    private readonly ILogger<UserLogController> _logger;

    public UserLogController(UserLogService userLogService, ILogger<UserLogController> logger)
    {
        _userLogService = userLogService;
        _logger = logger;
    }

    /// <summary>
    /// Log a login event when user clicks LOGIN AS MANAGER/ADMIN/USER buttons
    /// </summary>
    /// <param name="request">Login event request containing the event type</param>
    /// <returns>Response with generated user information and event details</returns>
    [HttpPost("login-event")]
    public async Task<ActionResult<LoginEventResponse>> LogLoginEvent([FromBody] LoginEventRequest request)
    {
        try
        {
            _logger.LogInformation("Logging login event for type: {EventType}", request.EventType);
            
            var response = await _userLogService.LogLoginEventAsync(request);
            
            if (!response.Success)
            {
                _logger.LogWarning("Failed to log login event: {Message}", response.Message);
                return BadRequest(response);
            }
            
            _logger.LogInformation("Successfully logged login event. ID: {Id}, User: {UserName}, Email: {UserId}", 
                response.Id, response.UserName, response.UserId);
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while logging login event for type: {EventType}", request.EventType);
            return StatusCode(500, new LoginEventResponse 
            { 
                Success = false, 
                Message = "Internal server error occurred while logging the event" 
            });
        }
    }

    /// <summary>
    /// Quick login endpoints for each user type (for easy frontend integration)
    /// </summary>
    [HttpPost("login-as-manager")]
    public async Task<ActionResult<LoginEventResponse>> LoginAsManager()
    {
        return await LogLoginEvent(new LoginEventRequest { EventType = "MANAGER" });
    }

    [HttpPost("login-as-admin")]
    public async Task<ActionResult<LoginEventResponse>> LoginAsAdmin()
    {
        return await LogLoginEvent(new LoginEventRequest { EventType = "ADMIN" });
    }

    [HttpPost("login-as-user")]
    public async Task<ActionResult<LoginEventResponse>> LoginAsUser()
    {
        return await LogLoginEvent(new LoginEventRequest { EventType = "USER" });
    }

    /// <summary>
    /// Get user logs with optional filtering and pagination
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 50)</param>
    /// <param name="eventType">Filter by event type (MANAGER, ADMIN, USER)</param>
    /// <returns>Paginated list of user logs</returns>
    [HttpGet]
    public async Task<ActionResult<UserLogListResponse>> GetUserLogs(
        [FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 50, 
        [FromQuery] string? eventType = null)
    {
        try
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 50;

            _logger.LogInformation("Retrieving user logs. Page: {PageNumber}, Size: {PageSize}, EventType: {EventType}", 
                pageNumber, pageSize, eventType);
            
            var response = await _userLogService.GetUserLogsAsync(pageNumber, pageSize, eventType);
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving user logs");
            return StatusCode(500, new UserLogListResponse());
        }
    }

    /// <summary>
    /// Get user logs by specific event type
    /// </summary>
    [HttpGet("by-event/{eventType}")]
    public async Task<ActionResult<UserLogListResponse>> GetUserLogsByEvent(
        string eventType,
        [FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 50)
    {
        return await GetUserLogs(pageNumber, pageSize, eventType);
    }

    /// <summary>
    /// Get user log statistics
    /// </summary>
    [HttpGet("stats")]
    public async Task<ActionResult<object>> GetUserLogStats()
    {
        try
        {
            var allLogs = await _userLogService.GetUserLogsAsync(1, int.MaxValue);
            
            var stats = new
            {
                TotalLogs = allLogs.TotalCount,
                ManagerLogins = allLogs.UserLogs.Count(ul => ul.Event == "MANAGER"),
                AdminLogins = allLogs.UserLogs.Count(ul => ul.Event == "ADMIN"),
                UserLogins = allLogs.UserLogs.Count(ul => ul.Event == "USER"),
                UniqueUsers = allLogs.UserLogs.DistinctBy(ul => ul.UserId).Count(),
                LastLogin = allLogs.UserLogs.OrderByDescending(ul => ul.EventDate).FirstOrDefault()?.EventDate,
                TopUsers = allLogs.UserLogs
                    .GroupBy(ul => ul.UserName)
                    .OrderByDescending(g => g.Count())
                    .Take(5)
                    .Select(g => new { UserName = g.Key, LoginCount = g.Count() })
                    .ToList()
            };
            
            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving user log statistics");
            return StatusCode(500, new { message = "Error retrieving statistics" });
        }
    }
}
