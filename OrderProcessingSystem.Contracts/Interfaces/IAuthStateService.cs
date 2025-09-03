namespace OrderProcessingSystem.Contracts.Interfaces;
public interface IAuthStateService
{
    event Action<bool>? AuthStateChanged;
    Task<bool> IsAuthenticatedAsync();
    Task<string?> GetUserNameAsync();
    Task<string?> GetUserRoleAsync();
    Task<string?> GetAccessTokenAsync();
    Task LogoutAsync();
    void NotifyAuthStateChanged();
}
