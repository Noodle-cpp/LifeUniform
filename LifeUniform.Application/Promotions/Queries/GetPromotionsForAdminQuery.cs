using LifeUniform.Application.Promotions.Dto;
using LifeUniform.Domain.Promotions;
using MediatR;

namespace LifeUniform.Application.Promotions.Queries;

public class GetPromotionsForAdminQuery : IRequest<IReadOnlyList<PromotionCodeDto>>;

public class GetPromotionsForAdminHandler : IRequestHandler<GetPromotionsForAdminQuery, IReadOnlyList<PromotionCodeDto>>
{
    private readonly IPromotionRepository _repo;

    public GetPromotionsForAdminHandler(IPromotionRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<PromotionCodeDto>> Handle(GetPromotionsForAdminQuery request, CancellationToken cancellationToken)
    {
        var list = await _repo.GetAllAsync(cancellationToken);
        return list.Select(p => new PromotionCodeDto
        {
            Id = p.Id,
            Code = p.Code,
            Type = (int)p.Type,
            Value = p.Value,
            MinOrderAmount = p.MinOrderAmount,
            ValidFrom = p.ValidFrom,
            ValidTo = p.ValidTo,
            IsActive = p.IsActive,
            MaxRedemptions = p.MaxRedemptions,
            RedemptionCount = p.RedemptionCount
        }).ToList();
    }
}
