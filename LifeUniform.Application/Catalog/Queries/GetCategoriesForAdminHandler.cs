using LifeUniform.Application.Catalog.Dto;
using LifeUniform.Application.Catalog.Mapping;
using LifeUniform.Domain.Catalog;
using MediatR;

namespace LifeUniform.Application.Catalog.Queries;

public class GetCategoriesForAdminHandler : IRequestHandler<GetCategoriesForAdminQuery, IReadOnlyList<CategoryAdminCardDto>>
{
    private readonly ICatalogRepository _catalogRepository;

    public GetCategoriesForAdminHandler(ICatalogRepository catalogRepository) => _catalogRepository = catalogRepository;

    public async Task<IReadOnlyList<CategoryAdminCardDto>> Handle(GetCategoriesForAdminQuery request, CancellationToken cancellationToken)
    {
        var categories = await _catalogRepository.GetCategoriesForAdminAsync(cancellationToken);
        return categories.Select(CatalogMapper.ToCategoryAdminCard).ToList();
    }
}
