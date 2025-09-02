using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using OrderProcessingSystem.Infrastructure;
using OrderProcessingSystem.Data;
using OrderProcessingSystem.Contracts.Interfaces;
using Microsoft.AspNetCore.ResponseCompression;
using OrderProcessingSystem.Events.FileWatcherTasks;
using OrderProcessingSystem.Core.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Configure ApplicationSettings first to get configuration paths
builder.Services.Configure<ApplicationSettings>(
    builder.Configuration.GetSection(ApplicationSettings.SectionName));

// Configure API Endpoints
builder.Services.Configure<ApiEndpointsConfiguration>(
    builder.Configuration.GetSection(ApiEndpointsConfiguration.SectionName));

// Get configuration paths from settings
var appSettings = new ApplicationSettings();
builder.Configuration.GetSection(ApplicationSettings.SectionName).Bind(appSettings);

// Load additional configuration files - file is linked and copied to output directory
string configPath = appSettings.JsonConfigurationPath;
if (!File.Exists(configPath))
{
    // Fallback to the bin directory path when running from project directory
    configPath = appSettings.JsonConfigurationFallbackPath;
}

builder.Configuration.AddJsonFile(configPath, optional: false, reloadOnChange: true);

// Configure options
builder.Services.Configure<BlobStorageSimulationOptions>(
    builder.Configuration.GetSection(BlobStorageSimulationOptions.SectionName));

builder.Services.Configure<FileNamingOptions>(
    builder.Configuration.GetSection(FileNamingOptions.SectionName));

// Add services
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
// HttpClient configuration
builder.Services.AddHttpClient();
// Named client for API
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri(appSettings.ApiClient.BaseAddress);
});
// Default scoped client still available
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient());

// AutoMapper - Add AutoMapper for DTO to ViewModel mappings
builder.Services.AddAutoMapper(typeof(Program));

// Register services - UI should use HTTP client instead of direct database access
// Use HTTP-based Infrastructure services for proper clean architecture
builder.Services.AddInfrastructureHttpServices();
builder.Services.AddScoped<IGridColumnService, OrderProcessingSystem.Infrastructure.Services.GridColumnService>();

// Register background services
builder.Services.AddSingleton<IBlobStorageMonitorService>(provider =>
{
    var logger = provider.GetRequiredService<ILogger<BlobStorageMonitorService>>();
    var options = provider.GetRequiredService<Microsoft.Extensions.Options.IOptions<BlobStorageSimulationOptions>>();
    var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
    var httpClient = httpClientFactory.CreateClient();
    return new BlobStorageMonitorService(logger, options, provider, httpClient);
});
builder.Services.AddHostedService(provider => provider.GetRequiredService<IBlobStorageMonitorService>());

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapRazorPages();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
