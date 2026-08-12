using LifeUniform.Application.Promotions.Dto;
using LifeUniform.Domain.Promotions;
using MediatR;

namespace LifeUniform.Application.Promotions.Queries;

public class GetPromotionByIdQuery : IRequest<PromotionCodeDto?>
{
    public Guid Id { get; init; }
}

public class GetPromotionByIdHandler : IRequestHandler<GetPromotionByIdQuery, PromotionCodeDto?>
{
    private readonly IPromotionRepository _repo;

    public GetPromotionByIdHandler(IPromotionRepository repo) => _repo = repo;

    public async Task<PromotionCodeDto?> Handle(GetPromotionByIdQuery request, CancellationToken cancellationToken)
    {
        var p = await _repo.GetByIdAsync(request.Id, cancellationToken);
        if (p is null)
            return null;

        return new PromotionCodeDto
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
        };
    }
}
