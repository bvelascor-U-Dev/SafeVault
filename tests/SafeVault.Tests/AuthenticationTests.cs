using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace SafeVault.Tests;

public class AuthenticationTests
{
    [Fact]
    public async Task LoginWithUnknownEmailReturnsPageWithError()
    {
        await using var dbContext = CreateDbContext();
        var page = CreateLoginModel(dbContext);
        page.LoginUser = new LoginUser
        {
            Email = "unknown@example.com",
            Password = "Password1!"
        };

        var result = await page.OnPostLoginAsync();

        Assert.IsType<PageResult>(result);
        Assert.False(page.ModelState.IsValid);
    }

    [Fact]
    public async Task LoginWithWrongPasswordReturnsPageWithError()
    {
        await using var dbContext = CreateDbContext();
        dbContext.Users.Add(CreateUser());
        await dbContext.SaveChangesAsync();

        var page = CreateLoginModel(dbContext);
        page.LoginUser = new LoginUser
        {
            Email = "admin@example.com",
            Password = "WrongPassword1!"
        };

        var result = await page.OnPostLoginAsync();

        Assert.IsType<PageResult>(result);
        Assert.False(page.ModelState.IsValid);
    }

    [Fact]
    public async Task LoginWithValidCredentialsCreatesAdminPrincipalAndRedirects()
    {
        await using var dbContext = CreateDbContext();
        dbContext.Users.Add(CreateUser());
        await dbContext.SaveChangesAsync();

        var authentication = new RecordingAuthenticationService();
        var page = CreateLoginModel(dbContext, authentication);
        page.LoginUser = new LoginUser
        {
            Email = "admin@example.com",
            Password = "Password1!"
        };

        var result = await page.OnPostLoginAsync();

        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Members", redirect.PageName);
        Assert.NotNull(authentication.Principal);
        Assert.True(authentication.Principal.IsInRole("Admin"));
    }

    private static SafeValueDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<SafeValueDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new SafeValueDbContext(options);
    }

    private static LoginModel CreateLoginModel(
        SafeValueDbContext dbContext,
        IAuthenticationService? authentication = null)
    {
        var services = new ServiceCollection();
        services.AddSingleton(authentication ?? new RecordingAuthenticationService());

        var page = new LoginModel(dbContext);
        page.PageContext.HttpContext = new DefaultHttpContext
        {
            RequestServices = services.BuildServiceProvider()
        };

        return page;
    }

    private static User CreateUser() => new()
    {
        Name = "Admin User",
        Email = "admin@example.com",
        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password1!"),
        Role = "Admin"
    };

    private sealed class RecordingAuthenticationService : IAuthenticationService
    {
        public System.Security.Claims.ClaimsPrincipal? Principal { get; private set; }

        public Task SignInAsync(
            HttpContext context,
            string? scheme,
            System.Security.Claims.ClaimsPrincipal principal,
            AuthenticationProperties? properties)
        {
            Principal = principal;
            return Task.CompletedTask;
        }

        public Task<AuthenticateResult> AuthenticateAsync(HttpContext context, string? scheme) =>
            Task.FromResult(AuthenticateResult.NoResult());

        public Task ChallengeAsync(
            HttpContext context,
            string? scheme,
            AuthenticationProperties? properties) => Task.CompletedTask;

        public Task ForbidAsync(
            HttpContext context,
            string? scheme,
            AuthenticationProperties? properties) => Task.CompletedTask;

        public Task SignOutAsync(
            HttpContext context,
            string? scheme,
            AuthenticationProperties? properties) => Task.CompletedTask;
    }
}
