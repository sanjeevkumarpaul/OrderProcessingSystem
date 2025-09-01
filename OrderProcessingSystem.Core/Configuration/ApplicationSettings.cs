namespace OrderProcessingSystem.Core.Configuration;

public class ApplicationSettings
{
    public const string SectionName = "ApplicationSettings";
    
    public string JsonConfigurationPath { get; set; } = string.Empty;
    public string JsonConfigurationFallbackPath { get; set; } = string.Empty;
    public string DatabasePath { get; set; } = string.Empty;
    public ApiClientSettings ApiClient { get; set; } = new();
}

public class ApiClientSettings
{
    public string BaseAddress { get; set; } = string.Empty;
}
