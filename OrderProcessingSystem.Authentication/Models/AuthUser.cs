namespace OrderProcessingSystem.Authentication.Models;

/// <summary>
/// Represents a user in the authentication system
/// </summary>
public class AuthUser
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    
    // OAuth Properties
    public string? Provider { get; set; }
    public string? ProviderId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    
    // Computed property for full name
    public string Name => !string.IsNullOrEmpty(DisplayName) ? DisplayName : $"{FirstName} {LastName}".Trim();
    
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; } = true;
    public List<string> Roles { get; set; } = new();
}
