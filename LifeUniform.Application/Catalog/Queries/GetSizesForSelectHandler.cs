using MediatR;
using LifeUniform.Domain.Catalog;
using LifeUniform.Application.Catalog.Dto;

namespace LifeUniform.Application.Catalog.Queries;

public class GetSizesForSelectHandler : IRequestHandler<GetSizesForSelectQuery, IReadOnlyList<SizeCardDto>>
{
    private readonly ICatalogRepository _catalogRepository;

    public GetSizesForSelectHandler(ICatalogRepository catalogRepository)
    {
        _catalogRepository = catalogRepository;
    }

    public async Task<IReadOnlyList<SizeCardDto>> Handle(GetSizesForSelectQuery request, CancellationToken cancellationToken)
    {
        var sizes = await _catalogRepository.GetAllSizesAsync(cancellationToken);
        return sizes.Select(s => new SizeCardDto { Id = s.Id, Label = s.Label }).ToList();
    }
}

