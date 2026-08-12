using FluentAssertions;
using LifeUniform.Application.Orders.Commands;
using LifeUniform.Domain.Cart;
using LifeUniform.Domain.Catalog;
using Moq;

namespace LifeUniform.Tests.Unit;

public class AddToCartHandlerTests
{
    [Fact]
    public async Task Handle_ProductWithSize_CallsCartAddOrUpdate()
    {
        var sizeId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var product = new Product
        {
            Id = productId,
            Name = "Medical Scrubs",
            Slug = "medical-scrubs",
            Price = 1500m,
            SizeOptions =
            {
                new ProductSizeOption
                {
                    ProductId = productId,
                    SizeId = sizeId,
                    Size = new Size { Id = sizeId, Label = "M", SortOrder = 2 }
                }
            }
        };

        var catalog = new Mock<ICatalogRepository>();
        catalog.Setup(c => c.GetProductBySlugAsync("medical-scrubs", It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var cart = new Mock<ICartService>();
        CartLine? added = null;
        cart.Setup(c => c.AddOrUpdate(It.IsAny<CartLine>()))
            .Callback<CartLine>(line => added = line);

        var sut = new AddToCartHandler(catalog.Object, cart.Object);

        await sut.Handle(new AddToCartCommand
        {
            ProductSlug = "medical-scrubs",
            SizeId = sizeId,
            Quantity = 2
        }, CancellationToken.None);

        cart.Verify(c => c.AddOrUpdate(It.IsAny<CartLine>()), Times.Once);
        added.Should().NotBeNull();
        added!.ProductId.Should().Be(productId);
        added.ProductSlug.Should().Be("medical-scrubs");
        added.SizeId.Should().Be(sizeId);
        added.SizeLabel.Should().Be("M");
        added.UnitPrice.Should().Be(1500m);
        added.Quantity.Should().Be(2);
    }
}
