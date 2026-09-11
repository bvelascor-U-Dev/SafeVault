using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddPolicy("LoginRateLimit", httpContext =>
    {
        if (!HttpMethods.IsPost(httpContext.Request.Method))
        {
            return RateLimitPartition.GetNoLimiter("login-get");
        }

        string clientIp =
            httpContext.Connection.RemoteIpAddress?.ToString()
            ?? "unknown";

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: $"login:{clientIp}",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
                AutoReplenishment = true
            });
    });

    options.OnRejected = async (context, cancellationToken) =>
    {
        await context.HttpContext.Response.WriteAsync(
            "Demasiados intentos de inicio de sesión. Intenta nuevamente en un minuto.",
            cancellationToken);
    };
});
builder.Services.AddScoped<EncryptionService>();
builder.Services.AddDbContext<SafeValueDbContext>(options =>
{
    string connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    options.UseNpgsql(connectionString);
});
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
   .AddCookie(options =>
   {
       options.LoginPath = "/login";
       options.AccessDeniedPath = "/login";
       options.ExpireTimeSpan = TimeSpan.FromMinutes(30);

       options.SlidingExpiration = true;
       options.Cookie.HttpOnly = true;
       options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
       options.Cookie.SameSite = SameSiteMode.Strict;
   });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();


app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
