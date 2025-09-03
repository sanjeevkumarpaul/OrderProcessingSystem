using OrderProcessingSystem.Authentication.Models;

namespace OrderProcessingSystem.Authentication.Interfaces;

/// <summary>
/// Simple token validation service for API authentication
/// Simulates JWT token validation for development/demo purposes
/// </summary>
public interface ITokenValidationService
{
    /// <summary>
    /// Validates if the provided token is valid
    /// </summary>
    /// <param name="token">The token to validate</param>
    /// <returns>True if token is valid, false otherwise</returns>
    bool ValidateToken(string token);

    /// <summary>
    /// Extracts user information from token
    /// </summary>
    /// <param name="token">The token to extract user info from</param>
    /// <returns>User information if token is valid, null otherwise</returns>
    TokenUserInfo? GetUserFromToken(string token);
}