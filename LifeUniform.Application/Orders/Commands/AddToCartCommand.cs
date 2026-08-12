using LifeUniform.Application.Catalog.Mapping;
using LifeUniform.Domain.Cart;
using LifeUniform.Domain.Catalog;
using MediatR;

namespace LifeUniform.Application.Orders.Commands;

public class AddToCartCommand : IRequest
{
    public string ProductSlug { get; init; } = string.Empty;
    public Guid SizeId { get; init; }
    public int Quantity { get; init; } = 1;
    public string? ColorName { get; init; }
}

public class AddToCartHandler : IRequestHandler<AddToCartCommand>
{
    private readonly ICatalogRepository _catalog;
    private readonly ICartService _cart;

    public AddToCartHandler(ICatalogRepository catalog, ICartService cart)
    {
        _catalog = catalog;
        _cart = cart;
    }

    public async Task Handle(AddToCartCommand request, CancellationToken cancellationToken)
    {
        var product = await _catalog.GetProductBySlugAsync(request.ProductSlug, cancellationToken)
            ?? throw new KeyNotFoundException($"Product not found: {request.ProductSlug}");

        if (!product.IsInStock)
            throw new InvalidOperationException("Товар сейчас недоступен.");

        var sizeOption = product.SizeOptions.FirstOrDefault(x => x.SizeId == request.SizeId)
            ?? throw new InvalidOperationException("Выбранный размер недоступен для этого товара.");

        var colorName = string.IsNullOrWhiteSpace(request.ColorName) ? null : request.ColorName.Trim();
        if (product.ColorOptions.Count > 0)
        {
            if (string.IsNullOrWhiteSpace(colorName))
                throw new InvalidOperationException("Выберите цвет товара.");

            var colorExists = product.ColorOptions.Any(c =>
                string.Equals(c.Name, colorName, StringComparison.OrdinalIgnoreCase));
            if (!colorExists)
                throw new InvalidOperationException("Выбранный цвет недоступен для этого товара.");
        }

        if (!IsColorSizeInStock(product, colorName, request.SizeId))
            throw new InvalidOperationException("Выбранный размер недоступен в этом цвете.");

        var qty = request.Quantity < 1 ? 1 : request.Quantity;
        var unitPrice = product.DiscountPrice ?? product.Price;
        var preview = CatalogMapper.ResolvePreviewUrl(product);
        if (!string.IsNullOrWhiteSpace(colorName))
        {
            var colorImage = product.ColorOptions
                .FirstOrDefault(c => string.Equals(c.Name, colorName, StringComparison.OrdinalIgnoreCase))
                ?.ImageFileName;
            var resolved = CatalogMapper.ResolveImageFile(colorImage);
            if (!string.IsNullOrWhiteSpace(resolved))
                preview = resolved;
        }

        _cart.AddOrUpdate(new CartLine
        {
            ProductId = product.Id,
            ProductName = product.Name,
            ProductSlug = product.Slug,
            PreviewImageUrl = preview,
            ColorName = colorName,
            SizeId = sizeOption.SizeId,
            SizeLabel = sizeOption.Size.Label,
            UnitPrice = unitPrice,
            Quantity = qty
        });
    }

    internal static bool IsColorSizeInStock(Product product, string? colorName, Guid sizeId)
    {
        // Legacy products without matrix: any product size is available.
        if (product.ColorSizeStocks.Count == 0)
            return product.SizeOptions.Any(s => s.SizeId == sizeId);

        if (string.IsNullOrWhiteSpace(colorName))
            return product.ColorSizeStocks.Any(s => s.SizeId == sizeId && s.IsInStock);

        return product.ColorSizeStocks.Any(s =>
            s.SizeId == sizeId
            && s.IsInStock
            && string.Equals(s.ColorName, colorName, StringComparison.OrdinalIgnoreCase));
    }
}

