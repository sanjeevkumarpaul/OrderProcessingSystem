using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.Text;
using OrderProcessingSystem.Authentication.Interfaces;
using OrderProcessingSystem.Authentication.Services;
using OrderProcessingSystem.Authentication.Models;
using IAuthService = OrderProcessingSystem.Authentication.Interfaces.IAuthenticationService;

namespace OrderProcessingSystem.Authentication.Extensions;

/// <summary>
/// Extension methods for configuring authentication services
/// </summary>
public static class AuthenticationServiceExtensions
{
    /// <summary>
    /// Add OAuth authentication services with Microsoft and Google providers
    /// </summary>
    public static IServiceCollection AddOAuthAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure OAuth settings from configuration
        var oAuthSettings = new OAuthSettings();
        configuration.GetSection(OAuthSettings.SectionName).Bind(oAuthSettings);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        })
        .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
        {
            options.LoginPath = "/login";
            options.LogoutPath = "/logout";
            options.AccessDeniedPath = "/access-denied";
            options.ExpireTimeSpan = TimeSpan.FromHours(24);
            options.SlidingExpiration = true;
        })
        .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
        {
            options.ClientId = oAuthSettings.Google.ClientId;
            options.ClientSecret = oAuthSettings.Google.ClientSecret;
            options.CallbackPath = "/signin-google";
            options.SaveTokens = true;
            
            // Request additional scopes
            options.Scope.Add("profile");
            options.Scope.Add("email");
        })
        .AddMicrosoftAccount(MicrosoftAccountDefaults.AuthenticationScheme, options =>
        {
            options.ClientId = oAuthSettings.Microsoft.ClientId;
            options.ClientSecret = oAuthSettings.Microsoft.ClientSecret;
            options.CallbackPath = "/signin-microsoft";
            options.SaveTokens = true;
            
            // Request additional scopes
            options.Scope.Add("User.Read");
        });

        return services;
    }

    /// <summary>
    /// Add JWT authentication services (for API tokens)
    /// </summary>
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, string secretKey, string issuer, string audience)
    {
        var key = Encoding.ASCII.GetBytes(secretKey);

        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(x =>
        {
            x.RequireHttpsMetadata = false;
            x.SaveToken = true;
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

        return services;
    }

    /// <summary>
    /// Add authentication services to the service collection
    /// </summary>
    public static IServiceCollection AddAuthenticationServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IUserService, UserService>();

        return services;
    }
}
