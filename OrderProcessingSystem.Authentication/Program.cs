using OrderProcessingSystem.Authentication.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Add HTTP client for API calls
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri("http://localhost:5269/");
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Add session support
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add OAuth Authentication
builder.Services.AddOAuthAuthentication(builder.Configuration);
builder.Services.AddAuthenticationServices();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();
app.UseSession(); // Add session middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
