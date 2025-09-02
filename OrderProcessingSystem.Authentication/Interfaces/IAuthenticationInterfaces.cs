using OrderProcessingSystem.Authentication.Models;

namespace OrderProcessingSystem.Authentication.Interfaces;

/// <summary>
/// Interface for authentication services
/// </summary>
public interface IAuthenticationService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
    Task<bool> LogoutAsync(string userId);
    Task<bool> ValidateTokenAsync(string token);
    Task<AuthUser?> GetUserByTokenAsync(string token);
    Task<string> GenerateTokenAsync(AuthUser user);
    Task<bool> RefreshTokenAsync(string refreshToken);
}

/// <summary>
/// Interface for user management services
/// </summary>
public interface IUserService
{
    Task<AuthUser?> GetUserByUsernameAsync(string username);
    Task<AuthUser?> GetUserByEmailAsync(string email);
    Task<AuthUser?> GetUserByIdAsync(int userId);
    Task<bool> CreateUserAsync(AuthUser user, string password);
    Task<bool> UpdateUserAsync(AuthUser user);
    Task<bool> DeleteUserAsync(int userId);
    Task<bool> VerifyPasswordAsync(AuthUser user, string password);
}
