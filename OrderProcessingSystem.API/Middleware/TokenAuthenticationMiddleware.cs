using OrderProcessingSystem.API.Services;

namespace OrderProcessingSystem.API.Middleware;

/// <summary>
/// Custom middleware to validate tokens for API requests
/// Simulates JWT authentication middleware
/// </summary>
public class TokenAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TokenAuthenticationMiddleware> _logger;

    public TokenAuthenticationMiddleware(RequestDelegate next, ILogger<TokenAuthenticationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, ITokenValidationService tokenValidationService)
    {
        // Skip authentication for certain paths
        var path = context.Request.Path.Value?.ToLower();
        if (ShouldSkipAuthentication(path))
        {
            await _next(context);
            return;
        }

        // Get the Authorization header
        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
        
        if (string.IsNullOrEmpty(authHeader))
        {
            _logger.LogWarning("API request to {Path} rejected: Missing Authorization header", path);
            await WriteUnauthorizedResponse(context, "Missing Authorization header");
            return;
        }

        // Validate the token
        if (!tokenValidationService.ValidateToken(authHeader))
        {
            _logger.LogWarning("API request to {Path} rejected: Invalid token", path);
            await WriteUnauthorizedResponse(context, "Invalid token");
            return;
        }

        // Add user info to context for controllers to access
        var userInfo = tokenValidationService.GetUserFromToken(authHeader);
        if (userInfo != null)
        {
            context.Items["User"] = userInfo;
            context.Items["Username"] = userInfo.Username;
            context.Items["Email"] = userInfo.Email;
            context.Items["Roles"] = userInfo.Roles;
            
            _logger.LogInformation("API request to {Path} authorized for user: {Username}", path, userInfo.Username);
        }

        await _next(context);
    }

    private static bool ShouldSkipAuthentication(string? path)
    {
        if (string.IsNullOrEmpty(path))
            return false;

        var skipPaths = new[]
        {
            "/swagger",
            "/health",
            "/error"
        };

        return skipPaths.Any(skipPath => path.StartsWith(skipPath, StringComparison.OrdinalIgnoreCase));
    }

    private static async Task WriteUnauthorizedResponse(HttpContext context, string message)
    {
        context.Response.StatusCode = 401;
        context.Response.ContentType = "application/json";

        var response = new
        {
            error = "Unauthorized",
            message = message,
            statusCode = 401,
            timestamp = DateTime.UtcNow
        };

        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
    }
}
