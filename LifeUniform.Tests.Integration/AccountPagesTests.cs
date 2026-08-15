using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace LifeUniform.Tests.Integration;

public class AccountPagesTests : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private readonly CustomWebApplicationFactory _factory;
    private HttpClient _client = null!;

    public AccountPagesTests(CustomWebApplicationFactory factory) => _factory = factory;

    public async Task InitializeAsync()
    {
        await _factory.EnsureDatabaseCreatedAsync();
        _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    public Task DisposeAsync()
    {
        _client.Dispose();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task AccountIndex_RequiresAuth()
    {
        var response = await _client.GetAsync("/Account");

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        response.Headers.Location!.ToString().Should().Contain("/Account/Auth");
    }

    [Fact]
    public async Task AccountProfile_RequiresAuth()
    {
        var response = await _client.GetAsync("/Account/Profile");

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        response.Headers.Location!.ToString().Should().Contain("/Account/Auth");
    }

    [Fact]
    public async Task Home_Guest_HasLoginNotAccountMenu()
    {
        var html = await _client.GetStringAsync("/");

        html.Should().Contain("loginModal");
        html.Should().NotContain("js-account-menu");
        html.Should().NotContain("Редактировать профиль");
    }
}
