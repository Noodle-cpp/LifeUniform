using LifeUniform.Application.Marketing.Dto;
using LifeUniform.Domain.Marketing;
using MediatR;

namespace LifeUniform.Application.Marketing.Queries;

public class GetActivePromoOffersQuery : IRequest<IReadOnlyList<PromoOfferDto>>;

public class GetActivePromoOffersHandler : IRequestHandler<GetActivePromoOffersQuery, IReadOnlyList<PromoOfferDto>>
{
    private readonly IPromoOfferRepository _repo;

    public GetActivePromoOffersHandler(IPromoOfferRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<PromoOfferDto>> Handle(GetActivePromoOffersQuery request, CancellationToken cancellationToken)
    {
        var list = await _repo.GetActiveAsync(cancellationToken);
        return list.Select(ToDto).ToList();
    }

    internal static PromoOfferDto ToDto(PromoOffer o) => new()
    {
        Id = o.Id,
        Title = o.Title,
        Subtitle = o.Subtitle,
        Badge = o.Badge,
        ImageUrl = o.ImageUrl,
        LinkUrl = o.LinkUrl,
        LinkText = o.LinkText,
        Price = o.Price,
        OldPrice = o.OldPrice,
        SortOrder = o.SortOrder,
        IsActive = o.IsActive
    };
}
