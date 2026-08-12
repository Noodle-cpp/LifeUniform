using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace LifeUniform.Tests.Integration;

public class CatalogPagesTests : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private readonly CustomWebApplicationFactory _factory;
    private HttpClient _client = null!;

    public CatalogPagesTests(CustomWebApplicationFactory factory) => _factory = factory;

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
    public async Task Home_ReturnsOk()
    {
        var response = await _client.GetAsync("/");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public async Task CatalogIndex_ReturnsOk()
    {
        var response = await _client.GetAsync("/Catalog");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public async Task CatalogIndex_ExplicitPath_ReturnsOk()
    {
        var response = await _client.GetAsync("/Catalog/Index");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public async Task Cart_ReturnsOk()
    {
        var response = await _client.GetAsync("/Cart");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public async Task Favorites_RequiresAuth()
    {
        var response = await _client.GetAsync("/Favorites");

        response.StatusCode.Should().BeOneOf(
            System.Net.HttpStatusCode.Redirect,
            System.Net.HttpStatusCode.Unauthorized,
            System.Net.HttpStatusCode.Found);
    }
}
