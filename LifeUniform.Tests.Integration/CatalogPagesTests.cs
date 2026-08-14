using FluentAssertions;
using LifeUniform.Application.Abstractions.Caching;
using LifeUniform.Domain.Catalog;
using LifeUniform.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

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
    public async Task CatalogSearch_ReturnsJson()
    {
        var response = await _client.GetAsync("/Catalog/Search?q=то");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
    }

    [Fact]
    public async Task Favorites_AllowsGuest()
    {
        var response = await _client.GetAsync("/Favorites");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public async Task CatalogIndex_WithoutGender_GroupsCategoriesByGender()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Categories.AddRange(
                new Category
                {
                    Id = Guid.NewGuid(),
                    Gender = ProductGender.Women,
                    Name = "Костюмы",
                    Slug = $"women-kostyumy-{Guid.NewGuid():N}",
                    SortOrder = 10
                },
                new Category
                {
                    Id = Guid.NewGuid(),
                    Gender = ProductGender.Men,
                    Name = "Костюмы",
                    Slug = $"men-kostyumy-{Guid.NewGuid():N}",
                    SortOrder = 10
                });
            await db.SaveChangesAsync();
        }

        var allHtml = System.Net.WebUtility.HtmlDecode(await _client.GetStringAsync("/Catalog"));
        allHtml.Should().Contain("Все модели");
        allHtml.Should().Contain("catalog-check__group-title");
        allHtml.Should().Contain("Женская одежда");
        allHtml.Should().Contain("Мужская одежда");

        var womenHtml = System.Net.WebUtility.HtmlDecode(await _client.GetStringAsync("/Catalog?gender=1"));
        womenHtml.Should().Contain("Женская одежда");
        womenHtml.Should().NotContain("Мужская одежда");
        womenHtml.Should().NotContain("catalog-check__group-title");
        womenHtml.Should().Contain("Костюмы");
    }

    [Fact]
    public async Task Home_RendersGenderCategoryDropdown()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Categories.AddRange(
                new Category
                {
                    Id = Guid.NewGuid(),
                    Gender = ProductGender.Women,
                    Name = "Халаты",
                    Slug = $"women-halaty-{Guid.NewGuid():N}",
                    SortOrder = 10
                },
                new Category
                {
                    Id = Guid.NewGuid(),
                    Gender = ProductGender.Men,
                    Name = "Брюки",
                    Slug = $"men-bryuki-{Guid.NewGuid():N}",
                    SortOrder = 10
                });
            await db.SaveChangesAsync();
            scope.ServiceProvider.GetRequiredService<IMemoryCache>().Remove(CatalogCacheKeys.Categories);
        }

        var html = System.Net.WebUtility.HtmlDecode(await _client.GetStringAsync("/"));
        html.Should().Contain("site-nav__dropdown");
        html.Should().Contain("Халаты");
        html.Should().Contain("Брюки");
        html.Should().Contain("js-nav-acc");
        html.Should().Contain("navWomenCats");
        html.Should().Contain("navMenCats");
    }
}
