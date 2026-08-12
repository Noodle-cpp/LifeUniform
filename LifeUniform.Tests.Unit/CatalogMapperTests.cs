using FluentAssertions;
using LifeUniform.Application.Catalog.Mapping;
using LifeUniform.Domain.Catalog;

namespace LifeUniform.Tests.Unit;

public class CatalogMapperTests
{
    [Fact]
    public void ToProductDetails_Maps_Material_And_Sizes()
    {
        var size = new Size { Id = Guid.NewGuid(), Label = "M" };
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Топ",
            Slug = "top-1",
            Material = "хлопок",
            Price = 1000m,
            SizeOptions =
            [
                new ProductSizeOption { SizeId = size.Id, Size = size }
            ]
        };

        var dto = CatalogMapper.ToProductDetails(product);

        dto.Material.Should().Be("хлопок");
        dto.Sizes.Should().ContainSingle(s => s.Label == "M");
        dto.PreviewImageUrl.Should().StartWith("/images/products/");
    }
}
