using Microsoft.AspNetCore.Mvc;
using OrderProcessingSystem.Authentication.Models;

namespace OrderProcessingSystem.API.Controllers.Base;

/// <summary>
/// Base controller that provides access to authenticated user information
/// </summary>
public abstract class AuthenticatedControllerBase : ControllerBase
{
    /// <summary>
    /// Gets the current authenticated user information from the token
    /// </summary>
    protected TokenUserInfo? CurrentUser => HttpContext.Items["User"] as TokenUserInfo;

    /// <summary>
    /// Gets the current username from the token
    /// </summary>
    protected string? CurrentUsername => HttpContext.Items["Username"] as string;

    /// <summary>
    /// Gets the current user email from the token
    /// </summary>
    protected string? CurrentUserEmail => HttpContext.Items["Email"] as string;

    /// <summary>
    /// Gets the current user roles from the token
    /// </summary>
    protected List<string>? CurrentUserRoles => HttpContext.Items["Roles"] as List<string>;

    /// <summary>
    /// Checks if the current user has a specific role
    /// </summary>
    /// <param name="role">The role to check</param>
    /// <returns>True if user has the role, false otherwise</returns>
    protected bool HasRole(string role)
    {
        return CurrentUserRoles?.Contains(role, StringComparer.OrdinalIgnoreCase) == true;
    }

    /// <summary>
    /// Checks if the current user is an admin
    /// </summary>
    protected bool IsAdmin => HasRole("Admin");

    /// <summary>
    /// Gets user information for logging purposes
    /// </summary>
    protected string UserContext => CurrentUser != null 
        ? $"{CurrentUser.Username} ({CurrentUser.Email})" 
        : "Anonymous";
}
