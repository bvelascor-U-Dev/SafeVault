using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace SafeVault.Tests;

public class AuthorizationTests
{
    [Fact]
    public async Task AdminRoleSatisfiesAdminOnlyPolicy()
    {
        using var services = CreateServices();
        var authorization = services.GetRequiredService<IAuthorizationService>();
        var admin = CreatePrincipal("Admin");

        var result = await authorization.AuthorizeAsync(
            admin,
            resource: null,
            policyName: "AdminOnly");

        Assert.True(result.Succeeded);
    }

    [Fact]
    public async Task UserRoleDoesNotSatisfyAdminOnlyPolicy()
    {
        using var services = CreateServices();
        var authorization = services.GetRequiredService<IAuthorizationService>();
        var user = CreatePrincipal("User");

        var result = await authorization.AuthorizeAsync(
            user,
            resource: null,
            policyName: "AdminOnly");

        Assert.False(result.Succeeded);
    }

    [Fact]
    public async Task AnonymousUserDoesNotSatisfyAdminOnlyPolicy()
    {
        using var services = CreateServices();
        var authorization = services.GetRequiredService<IAuthorizationService>();
        var anonymous = new ClaimsPrincipal(new ClaimsIdentity());

        var result = await authorization.AuthorizeAsync(
            anonymous,
            resource: null,
            policyName: "AdminOnly");

        Assert.False(result.Succeeded);
    }

    private static ServiceProvider CreateServices()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy =>
            {
                policy.RequireRole("Admin");
            });
        });

        return services.BuildServiceProvider();
    }

    private static ClaimsPrincipal CreatePrincipal(string role)
    {
        var identity = new ClaimsIdentity(
            new[]
            {
                new Claim(ClaimTypes.Name, "Test User"),
                new Claim(ClaimTypes.Role, role)
            },
            authenticationType: "Test");

        return new ClaimsPrincipal(identity);
    }
}
