using MediatR;
using LifeUniform.Application.Abstractions.Caching;
using LifeUniform.Domain.Catalog;

namespace LifeUniform.Application.Catalog.Commands;

public class UpsertCategoryHandler : IRequestHandler<UpsertCategoryCommand>
{
    private readonly ICatalogRepository _catalogRepository;
    private readonly ICatalogCacheInvalidator _cacheInvalidator;

    public UpsertCategoryHandler(ICatalogRepository catalogRepository, ICatalogCacheInvalidator cacheInvalidator)
    {
        _catalogRepository = catalogRepository;
        _cacheInvalidator = cacheInvalidator;
    }

    public async Task Handle(UpsertCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = new Category
        {
            Slug = request.Slug,
            Name = request.Name,
            Gender = request.Gender,
            Description = request.Description,
            SortOrder = request.SortOrder,
            IsActive = request.IsActive
        };

        await _catalogRepository.UpsertCategoryAsync(category, cancellationToken);
        _cacheInvalidator.InvalidateCatalog();
    }
}
