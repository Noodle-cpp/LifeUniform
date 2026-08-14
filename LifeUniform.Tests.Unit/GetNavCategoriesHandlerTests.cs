using FluentAssertions;
using LifeUniform.Application.Catalog.Queries;
using LifeUniform.Domain.Catalog;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace LifeUniform.Tests.Unit;

public class GetNavCategoriesHandlerTests
{
    [Fact]
    public async Task Handle_SplitsCategoriesByGender()
    {
        var womenSuits = new Category
        {
            Id = Guid.NewGuid(),
            Gender = ProductGender.Women,
            Name = "Костюмы",
            Slug = "women-kostyumy",
            IsActive = true
        };
        var menTops = new Category
        {
            Id = Guid.NewGuid(),
            Gender = ProductGender.Men,
            Name = "Топы",
            Slug = "men-topy",
            IsActive = true
        };

        var catalog = new Mock<ICatalogRepository>();
        catalog.Setup(c => c.GetCategoriesAsync(null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { womenSuits, menTops });

        var sut = new GetNavCategoriesHandler(catalog.Object, new MemoryCache(new MemoryCacheOptions()));

        var result = await sut.Handle(new GetNavCategoriesQuery(), CancellationToken.None);

        result.Women.Should().ContainSingle(c => c.Name == "Костюмы");
        result.Men.Should().ContainSingle(c => c.Name == "Топы");
    }
}
