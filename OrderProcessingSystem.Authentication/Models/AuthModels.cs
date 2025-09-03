namespace OrderProcessingSystem.Authentication.Models;

/// <summary>
/// Login request model
/// </summary>
public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool RememberMe { get; set; }
}

/// <summary>
/// Login response model
/// </summary>
public class LoginResponse
{
    public bool Success { get; set; }
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public string Message { get; set; } = string.Empty;
    public AuthUser? User { get; set; }
}

/// <summary>
/// Demo token request model
/// </summary>
public class DemoTokenRequest
{
    public string Provider { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

/// <summary>
/// Demo token response model
/// </summary>
public class DemoTokenResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string TokenType { get; set; } = "Bearer";
    public int ExpiresIn { get; set; }
    public AuthUser User { get; set; } = new();
    public DateTime IssuedAt { get; set; }
}
