using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using OrderProcessingSystem.Infrastructure.Services;
using OrderProcessingSystem.Contracts.Interfaces;
using OrderProcessingServer.Services;

var builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddScoped<DataLoadingService>(provider =>
{
    var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
    var logger = provider.GetRequiredService<ILogger<DataLoadingService>>();
    return new DataLoadingService(httpClientFactory, logger);
});

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
