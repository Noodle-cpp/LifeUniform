using MediatR;
using LifeUniform.Domain.Catalog;
using LifeUniform.Application.Catalog.Dto;

namespace LifeUniform.Application.Catalog.Queries;

public class GetCategoriesForSelectHandler : IRequestHandler<GetCategoriesForSelectQuery, IReadOnlyList<CategorySelectDto>>
{
    private readonly ICatalogRepository _catalogRepository;

    public GetCategoriesForSelectHandler(ICatalogRepository catalogRepository)
    {
        _catalogRepository = catalogRepository;
    }

    public async Task<IReadOnlyList<CategorySelectDto>> Handle(GetCategoriesForSelectQuery request, CancellationToken cancellationToken)
    {
        var categories = await _catalogRepository.GetCategoriesForAdminAsync(cancellationToken);
        return categories.Select(c => new CategorySelectDto { Id = c.Id, Name = c.Name }).ToList();
    }
}

