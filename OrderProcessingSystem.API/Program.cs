using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;
using OrderProcessingSystem.Data;
using OrderProcessingSystem.Infrastructure;
using OrderProcessingSystem.Cache;
using OrderProcessingSystem.Contracts.Interfaces;
using OrderProcessingSystem.API.Services;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Register controllers
builder.Services.AddControllers();

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// AutoMapper - Register all mapping profiles from this assembly
// Fix for AutoMapper version compatibility issue
builder.Services.AddAutoMapper(typeof(Program));

// Register mapping services
builder.Services.AddScoped<IGridColumnMappingService, GridColumnMappingService>();

// Register metadata services
builder.Services.AddScoped<IGridMetadataService,GridMetadataService>();

// Register Data services with SQLite file in OrderDatabase/OPSDB.db
var dbPath = Path.Combine(builder.Environment.ContentRootPath, "..", "OrderDatabase", "OPSDB.db");
var conn = $"Data Source={dbPath}";
builder.Services.AddOrderProcessingData(conn);
// Register infrastructure services (OrderService, repositories)
builder.Services.AddInfrastructureServices();

// Add caching services
builder.Services.AddOrderProcessingCache(builder.Configuration);

var app = builder.Build();

// Enable Swagger (OpenAPI) UI
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.RoutePrefix = "swagger"; // serve at /swagger
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "OrderProcessingSystem API V1");
});

// Seed database on startup
using (var scope = app.Services.CreateScope())
{
    var sp = scope.ServiceProvider;
    try
    {
        var db = sp.GetRequiredService<AppDbContext>();
        await DataSeeder.SeedAsync(db);
    }
    catch (Exception ex)
    {
        var logger = sp.GetService<ILoggerFactory>()?.CreateLogger("Startup");
        logger?.LogError(ex, "Error seeding database");
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapControllers();

app.Run();
