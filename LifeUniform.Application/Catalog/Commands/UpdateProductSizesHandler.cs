using MediatR;
using LifeUniform.Domain.Catalog;

namespace LifeUniform.Application.Catalog.Commands;

public class UpdateProductSizesHandler : IRequestHandler<UpdateProductSizesCommand>
{
    private readonly ICatalogRepository _catalogRepository;

    public UpdateProductSizesHandler(ICatalogRepository catalogRepository)
    {
        _catalogRepository = catalogRepository;
    }

    public async Task Handle(UpdateProductSizesCommand request, CancellationToken cancellationToken)
    {
        var product = await _catalogRepository.GetProductWithSizesBySlugAsync(request.Slug, cancellationToken);
        if (product is null)
            throw new KeyNotFoundException($"Product not found: {request.Slug}");

        await _catalogRepository.SetProductSizesAsync(product.Id, request.SizeIds, cancellationToken);
    }
}

