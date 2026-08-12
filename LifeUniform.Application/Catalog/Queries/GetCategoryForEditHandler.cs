using LifeUniform.Application.Catalog.Dto;
using LifeUniform.Application.Catalog.Mapping;
using LifeUniform.Domain.Catalog;
using MediatR;

namespace LifeUniform.Application.Catalog.Queries;

public class GetCategoryForEditHandler : IRequestHandler<GetCategoryForEditQuery, CategoryEditDto?>
{
    private readonly ICatalogRepository _catalogRepository;

    public GetCategoryForEditHandler(ICatalogRepository catalogRepository) => _catalogRepository = catalogRepository;

    public async Task<CategoryEditDto?> Handle(GetCategoryForEditQuery request, CancellationToken cancellationToken)
    {
        var category = await _catalogRepository.GetCategoryBySlugAsync(request.Slug, cancellationToken);
        return category is null ? null : CatalogMapper.ToCategoryEdit(category);
    }
}
