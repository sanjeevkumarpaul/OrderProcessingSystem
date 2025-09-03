namespace OrderProcessingSystem.UI.Models;

/// <summary>
/// Demo token response model from Authentication service
/// </summary>
public class DemoTokenResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string TokenType { get; set; } = "Bearer";
    public int ExpiresIn { get; set; }
    public AuthUserResponse User { get; set; } = new();
    public DateTime IssuedAt { get; set; }
}

/// <summary>
/// User information from Authentication service
/// </summary>
public class AuthUserResponse
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    public string ProviderId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; }
    public List<string> Roles { get; set; } = new();
}
