namespace OrderProcessingSystem.Authentication.Models;

/// <summary>
/// OAuth configuration settings
/// </summary>
public class OAuthSettings
{
    public const string SectionName = "OAuth";
    
    public MicrosoftSettings Microsoft { get; set; } = new();
    public GoogleSettings Google { get; set; } = new();
}

public class MicrosoftSettings
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
}

public class GoogleSettings
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
}
