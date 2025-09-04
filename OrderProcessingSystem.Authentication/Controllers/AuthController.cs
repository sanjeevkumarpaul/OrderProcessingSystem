using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Mvc;
using OrderProcessingSystem.Authentication.Models;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace OrderProcessingSystem.Authentication.Controllers;

[Route("auth")]
public class AuthController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IConfiguration configuration, IHttpClientFactory httpClientFactory, ILogger<AuthController> logger)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    /// <summary>
    /// Health check endpoint for authentication service
    /// </summary>
    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new { status = "Authentication service is running", timestamp = DateTime.UtcNow });
    }

    /// <summary>
    /// Demo login with Microsoft (bypasses real OAuth)
    /// </summary>
    [HttpGet("demo/microsoft")]
    public async Task<IActionResult> DemoMicrosoftLogin()
    {
        var demoUser = new AuthUser
        {
            Id = 1,
            Email = "sanjeev.p@microsoft.com",
            DisplayName = "Sanjeev P",
            FirstName = "Sanjeev",
            LastName = "P",
            Provider = "Microsoft",
            ProviderId = "demo-microsoft-123",
            CreatedAt = DateTime.UtcNow,
            Username = "sanjeev.p"
        };

        // Store user in session/cache
        HttpContext.Session.SetString("AuthenticatedUser", System.Text.Json.JsonSerializer.Serialize(demoUser));
        
        // Sign in the user with cookie authentication
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, demoUser.ProviderId!),
            new Claim(ClaimTypes.Name, demoUser.Name),
            new Claim(ClaimTypes.Email, demoUser.Email),
            new Claim("Provider", demoUser.Provider!)
        };

        var claimsIdentity = new ClaimsIdentity(claims, "Demo");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        
        await HttpContext.SignInAsync("Cookies", claimsPrincipal);

        // Log the login event to the API for real-time notifications
        await LogLoginEventAsync(demoUser.Name, "ADMIN");

        return Redirect($"{GetUIBaseUrl()}/auth-success?provider=microsoft&demo=true");
    }

    /// <summary>
    /// Demo login with Google (bypasses real OAuth)
    /// </summary>
    [HttpGet("demo/google")]
    public async Task<IActionResult> DemoGoogleLogin()
    {
        var demoUser = new AuthUser
        {
            Id = 2,
            Email = "sanjeev.p@gmail.com",
            DisplayName = "Sanjeev P",
            FirstName = "Sanjeev",
            LastName = "P",
            Provider = "Google",
            ProviderId = "demo-google-456",
            CreatedAt = DateTime.UtcNow,
            Username = "sanjeev.p"
        };

        // Store user in session/cache
        HttpContext.Session.SetString("AuthenticatedUser", System.Text.Json.JsonSerializer.Serialize(demoUser));
        
        // Sign in the user with cookie authentication
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, demoUser.ProviderId!),
            new Claim(ClaimTypes.Name, demoUser.Name),
            new Claim(ClaimTypes.Email, demoUser.Email),
            new Claim("Provider", demoUser.Provider!)
        };

        var claimsIdentity = new ClaimsIdentity(claims, "Demo");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        
        await HttpContext.SignInAsync("Cookies", claimsPrincipal);

        // Log the login event to the API for real-time notifications
        await LogLoginEventAsync(demoUser.Name, "MANAGER");

        return Redirect($"{GetUIBaseUrl()}/auth-success?provider=google&demo=true");
    }

    /// <summary>
    /// Initiate Google OAuth login
    /// </summary>
    [HttpGet("google")]
    public IActionResult GoogleLogin()
    {
        var googleClientId = _configuration["OAuth:Google:ClientId"];
        if (string.IsNullOrEmpty(googleClientId) || googleClientId.Contains("your-google-client-id"))
        {
            return BadRequest(new { 
                error = "OAuth Configuration Missing", 
                message = "Google OAuth credentials are not configured. Please set up OAuth:Google:ClientId and OAuth:Google:ClientSecret in appsettings.json",
                instructions = "Visit https://console.cloud.google.com to create OAuth 2.0 credentials"
            });
        }

        var redirectUrl = Url.Action(nameof(GoogleCallback), "Auth");
        var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    /// <summary>
    /// Google OAuth callback
    /// </summary>
    [HttpGet("google-callback")]
    public async Task<IActionResult> GoogleCallback()
    {
        var authenticateResult = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
        
        if (!authenticateResult.Succeeded)
        {
            return BadRequest("Google authentication failed");
        }

        var user = CreateAuthUserFromClaims(authenticateResult.Principal!, "Google");
        
        // Store user info in session/cookie for the UI to access
        await StoreUserInSession(user);
        
        // Redirect to UI with success indication
        return Redirect($"{GetUIBaseUrl()}/auth-success?provider=google");
    }

    /// <summary>
    /// Initiate Microsoft OAuth login
    /// </summary>
    [HttpGet("microsoft")]
    public IActionResult MicrosoftLogin()
    {
        var microsoftClientId = _configuration["OAuth:Microsoft:ClientId"];
        if (string.IsNullOrEmpty(microsoftClientId) || microsoftClientId.Contains("your-microsoft-client-id"))
        {
            return BadRequest(new { 
                error = "OAuth Configuration Missing", 
                message = "Microsoft OAuth credentials are not configured. Please set up OAuth:Microsoft:ClientId and OAuth:Microsoft:ClientSecret in appsettings.json",
                instructions = "Visit https://portal.azure.com to create an App Registration"
            });
        }

        var redirectUrl = Url.Action(nameof(MicrosoftCallback), "Auth");
        var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
        return Challenge(properties, MicrosoftAccountDefaults.AuthenticationScheme);
    }

    /// <summary>
    /// Microsoft OAuth callback
    /// </summary>
    [HttpGet("microsoft-callback")]
    public async Task<IActionResult> MicrosoftCallback()
    {
        var authenticateResult = await HttpContext.AuthenticateAsync(MicrosoftAccountDefaults.AuthenticationScheme);
        
        if (!authenticateResult.Succeeded)
        {
            return BadRequest("Microsoft authentication failed");
        }

        var user = CreateAuthUserFromClaims(authenticateResult.Principal!, "Microsoft");
        
        // Store user info in session/cookie for the UI to access
        await StoreUserInSession(user);
        
        // Redirect to UI with success indication
        return Redirect($"{GetUIBaseUrl()}/auth-success?provider=microsoft");
    }

    /// <summary>
    /// Logout endpoint
    /// </summary>
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return Ok(new { success = true, message = "Logged out successfully" });
    }

    /// <summary>
    /// Get current user info
    /// </summary>
    [HttpGet("user")]
    public async Task<IActionResult> GetUser()
    {
        var authenticateResult = await HttpContext.AuthenticateAsync();
        
        if (!authenticateResult.Succeeded || authenticateResult.Principal == null)
        {
            return Unauthorized();
        }

        var user = CreateAuthUserFromClaims(authenticateResult.Principal, "");
        return Ok(user);
    }

    private AuthUser CreateAuthUserFromClaims(ClaimsPrincipal principal, string provider)
    {
        var nameIdentifier = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
        var email = principal.FindFirst(ClaimTypes.Email)?.Value ?? "";
        var name = principal.FindFirst(ClaimTypes.Name)?.Value ?? email;
        var givenName = principal.FindFirst(ClaimTypes.GivenName)?.Value ?? "";
        var surname = principal.FindFirst(ClaimTypes.Surname)?.Value ?? "";

        return new AuthUser
        {
            Id = nameIdentifier.GetHashCode(),
            Username = name,
            Email = email,
            FirstName = givenName,
            LastName = surname,
            CreatedAt = DateTime.UtcNow,
            LastLoginAt = DateTime.UtcNow,
            IsActive = true,
            Roles = new List<string> { "User" }
        };
    }

    private async Task StoreUserInSession(AuthUser user)
    {
        // Create claims for the authenticated user
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.GivenName, user.FirstName),
            new(ClaimTypes.Surname, user.LastName)
        };

        foreach (var role in user.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var claimsIdentity = new ClaimsIdentity(claims, "OAuth");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        await HttpContext.SignInAsync(claimsPrincipal);
    }

    private async Task LogLoginEventAsync(string userName, string userRole = "USER")
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient("ApiClient");
            var loginRequest = new
            {
                EventType = userRole
            };

            var json = JsonSerializer.Serialize(loginRequest);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            _logger.LogInformation("🔔 Logging login event for user: {UserName} with role: {Role}", userName, userRole);
            
            var response = await httpClient.PostAsync("api/UserLog/login-event", content);
            
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("✅ Successfully logged login event for user: {UserName}", userName);
            }
            else
            {
                _logger.LogWarning("⚠️ Failed to log login event for user: {UserName}. Status: {Status}", userName, response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error logging login event for user: {UserName}", userName);
        }
    }

    private string GetUIBaseUrl()
    {
        // In production, this should come from configuration
        return "http://localhost:5253";
    }
}
