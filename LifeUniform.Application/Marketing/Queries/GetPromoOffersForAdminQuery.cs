using LifeUniform.Application.Marketing.Dto;
using LifeUniform.Domain.Marketing;
using MediatR;

namespace LifeUniform.Application.Marketing.Queries;

public class GetPromoOffersForAdminQuery : IRequest<IReadOnlyList<PromoOfferDto>>;

public class GetPromoOffersForAdminHandler : IRequestHandler<GetPromoOffersForAdminQuery, IReadOnlyList<PromoOfferDto>>
{
    private readonly IPromoOfferRepository _repo;

    public GetPromoOffersForAdminHandler(IPromoOfferRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<PromoOfferDto>> Handle(GetPromoOffersForAdminQuery request, CancellationToken cancellationToken)
    {
        var list = await _repo.GetAllAsync(cancellationToken);
        return list.Select(GetActivePromoOffersHandler.ToDto).ToList();
    }
}
