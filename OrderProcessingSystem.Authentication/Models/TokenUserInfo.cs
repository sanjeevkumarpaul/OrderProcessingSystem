
namespace OrderProcessingSystem.Authentication.Models;
/// <summary>
/// User information extracted from token
/// </summary>
public class TokenUserInfo
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
}