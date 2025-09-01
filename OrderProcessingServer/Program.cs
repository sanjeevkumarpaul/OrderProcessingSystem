using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using OrderProcessingSystem.Infrastructure.Services;
using OrderProcessingSystem.Contracts.Interfaces;
using OrderProcessingServer.Services;
using OrderProcessingSystem.Events.FileWatcherTasks;
using OrderProcessingSystem.Core.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Load additional configuration files
builder.Configuration.AddJsonFile("Configuration/file-naming.json", optional: false, reloadOnChange: true);

// Configure options
builder.Services.Configure<BlobStorageSimulationOptions>(
    builder.Configuration.GetSection(BlobStorageSimulationOptions.SectionName));

builder.Services.Configure<FileNamingOptions>(
    builder.Configuration.GetSection(FileNamingOptions.SectionName));

// Add services
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
// Register HttpClient so components can @inject HttpClient
builder.Services.AddHttpClient();
// Named client for API
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri("http://localhost:5269/");
});
// Default scoped client still available
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient());

// Register services
builder.Services.AddScoped<IGridColumnService, GridColumnService>();
builder.Services.AddScoped<OrderFileService>();
builder.Services.AddScoped(provider =>
{
    var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
    var logger = provider.GetRequiredService<ILogger<DataLoadingService>>();
    return new DataLoadingService(httpClientFactory, logger);
});

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
