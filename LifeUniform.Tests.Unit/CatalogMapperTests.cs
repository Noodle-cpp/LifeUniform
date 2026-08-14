using FluentAssertions;
using LifeUniform.Application.Catalog.Dto;
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
        dto.PreviewImageUrl.Should().StartWith("/images/");
    }

    [Fact]
    public void ToProductCard_Maps_Sizes_And_Snippet()
    {
        var size = new Size { Id = Guid.NewGuid(), Label = "L", SortOrder = 2 };
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Костюм",
            Slug = "suit-1",
            Description = "Таблица размеров и посадка по фигуре",
            Price = 7200m,
            SizeOptions =
            [
                new ProductSizeOption { SizeId = size.Id, Size = size }
            ],
            ColorOptions =
            [
                new ProductColorOption { Name = "Серый", Hex = "#999999" }
            ]
        };

        var dto = CatalogMapper.ToProductCard(product);

        dto.Sizes.Should().ContainSingle(s => s.Label == "L");
        dto.Snippet.Should().Contain("Таблица размеров");
        dto.Colors.Should().ContainSingle(c => c.Name == "Серый");
    }

    [Fact]
    public void GroupCategoriesByGender_SplitsDuplicateNames()
    {
        var womenSuits = new CategoryCardDto
        {
            Id = Guid.NewGuid(),
            Name = "Костюмы",
            Slug = "women-kostyumy",
            Gender = (int)ProductGender.Women
        };
        var menSuits = new CategoryCardDto
        {
            Id = Guid.NewGuid(),
            Name = "Костюмы",
            Slug = "men-kostyumy",
            Gender = (int)ProductGender.Men
        };
        var womenTops = new CategoryCardDto
        {
            Id = Guid.NewGuid(),
            Name = "Топы",
            Slug = "women-topy",
            Gender = (int)ProductGender.Women
        };

        var groups = CatalogMapper.GroupCategoriesByGender([womenSuits, menSuits, womenTops]);

        groups.Should().HaveCount(2);
        groups[0].Title.Should().Be("Женская одежда");
        groups[0].Items.Select(c => c.Name).Should().Equal("Костюмы", "Топы");
        groups[1].Title.Should().Be("Мужская одежда");
        groups[1].Items.Select(c => c.Name).Should().Equal("Костюмы");
    }
}
