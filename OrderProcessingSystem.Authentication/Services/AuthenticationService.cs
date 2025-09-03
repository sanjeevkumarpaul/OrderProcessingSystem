using OrderProcessingSystem.Authentication.Interfaces;
using OrderProcessingSystem.Authentication.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using IAuthService = OrderProcessingSystem.Authentication.Interfaces.IAuthenticationService;

namespace OrderProcessingSystem.Authentication.Services;

/// <summary>
/// Service for handling OAuth authentication operations
/// </summary>
public class AuthenticationService : IAuthService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AuthenticationService> _logger;

    public AuthenticationService(
        IHttpContextAccessor httpContextAccessor,
        ILogger<AuthenticationService> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        try
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                return new LoginResponse 
                { 
                    Success = false, 
                    Message = "HTTP context not available" 
                };
            }

            // Check if user is already authenticated from OAuth callback
            var authenticateResult = await httpContext.AuthenticateAsync();
            if (!authenticateResult.Succeeded || authenticateResult.Principal == null)
            {
                return new LoginResponse 
                { 
                    Success = false, 
                    Message = "Authentication failed" 
                };
            }

            var user = CreateAuthUserFromClaims(authenticateResult.Principal);
            var token = await GenerateTokenAsync(user);

            return new LoginResponse
            {
                Success = true,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddHours(24),
                Message = "Authentication successful",
                User = user
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login failed");
            return new LoginResponse 
            { 
                Success = false, 
                Message = $"Login failed: {ex.Message}" 
            };
        }
    }

    public async Task<bool> LogoutAsync(string userId)
    {
        try
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                await httpContext.SignOutAsync();
            }
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Logout failed for user {UserId}", userId);
            return false;
        }
    }

    public Task<bool> ValidateTokenAsync(string token)
    {
        // TODO: Implement JWT token validation
        // For now, simple check for non-empty token
        return Task.FromResult(!string.IsNullOrEmpty(token));
    }

    public Task<AuthUser?> GetUserByTokenAsync(string token)
    {
        try
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated == true)
            {
                return Task.FromResult<AuthUser?>(CreateAuthUserFromClaims(httpContext.User));
            }
            return Task.FromResult<AuthUser?>(null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get user by token");
            return Task.FromResult<AuthUser?>(null);
        }
    }

    public Task<string> GenerateTokenAsync(AuthUser user)
    {
        // For demonstration, create a simple token
        // In production, you'd use JWT with proper signing
        var token = $"{user.Username}_{DateTime.UtcNow.Ticks}_{Guid.NewGuid():N}";
        return Task.FromResult(token);
    }

    public Task<bool> RefreshTokenAsync(string refreshToken)
    {
        // TODO: Implement refresh token logic
        return Task.FromResult(true);
    }

    private AuthUser CreateAuthUserFromClaims(ClaimsPrincipal principal)
    {
        var nameIdentifier = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
        var email = principal.FindFirst(ClaimTypes.Email)?.Value ?? "";
        var name = principal.FindFirst(ClaimTypes.Name)?.Value ?? email;
        var givenName = principal.FindFirst(ClaimTypes.GivenName)?.Value ?? "";
        var surname = principal.FindFirst(ClaimTypes.Surname)?.Value ?? "";

        return new AuthUser
        {
            Id = nameIdentifier.GetHashCode(), // Simple ID generation
            Username = name,
            Email = email,
            FirstName = givenName,
            LastName = surname,
            CreatedAt = DateTime.UtcNow,
            LastLoginAt = DateTime.UtcNow,
            IsActive = true,
            Roles = new List<string> { "User" } // Default role
        };
    }
}
