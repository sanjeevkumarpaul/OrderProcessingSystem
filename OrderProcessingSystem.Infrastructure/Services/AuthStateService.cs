using Microsoft.JSInterop;
using OrderProcessingSystem.Contracts.Interfaces;

namespace OrderProcessingSystem.Infrastructure.Services;

/// <summary>
/// Service to manage authentication state in the UI
/// </summary>

public class AuthStateService : IAuthStateService
{
    private readonly IJSRuntime _jsRuntime;
    
    public event Action<bool>? AuthStateChanged;

    public AuthStateService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        try
        {
            var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "accessToken");
            var loginTimeStr = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "loginTime");

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(loginTimeStr))
                return false;

            // Check if token is still valid (24 hours expiry)
            if (DateTime.TryParse(loginTimeStr, out var loginTime))
            {
                if (DateTime.UtcNow.Subtract(loginTime).TotalHours < 24)
                {
                    return true;
                }
                else
                {
                    // Token expired, clear storage
                    await LogoutAsync();
                    return false;
                }
            }

            return false;
        }
        catch
        {
            return false;
        }
    }

    public async Task<string?> GetUserNameAsync()
    {
        try
        {
            return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "userName");
        }
        catch
        {
            return null;
        }
    }

    public async Task<string?> GetUserRoleAsync()
    {
        try
        {
            return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "userRole");
        }
        catch
        {
            return null;
        }
    }

    public async Task LogoutAsync()
    {
        try
        {
            // Clear all authentication data
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authProvider");
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "accessToken");
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "userName");
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "userEmail");
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "userRole");
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "loginTime");
            
            NotifyAuthStateChanged();
        }
        catch
        {
            // Ignore errors during logout
        }
    }

    public void NotifyAuthStateChanged()
    {
        AuthStateChanged?.Invoke(true);
    }
}
