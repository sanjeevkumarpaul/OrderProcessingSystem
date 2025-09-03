using OrderProcessingSystem.Authentication.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

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
