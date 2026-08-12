using FluentValidation;
using LifeUniform.Domain.Promotions;
using MediatR;

namespace LifeUniform.Application.Promotions.Commands;

public class UpsertPromotionCommand : IRequest
{
    public Guid? Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public PromotionDiscountType Type { get; init; }
    public decimal Value { get; init; }
    public decimal? MinOrderAmount { get; init; }
    public DateTimeOffset? ValidFrom { get; init; }
    public DateTimeOffset? ValidTo { get; init; }
    public bool IsActive { get; init; } = true;
    public int? MaxRedemptions { get; init; }
}

public class UpsertPromotionCommandValidator : AbstractValidator<UpsertPromotionCommand>
{
    public UpsertPromotionCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.Value).GreaterThan(0);
        RuleFor(x => x.Value).LessThanOrEqualTo(100).When(x => x.Type == PromotionDiscountType.Percent);
    }
}

public class UpsertPromotionHandler : IRequestHandler<UpsertPromotionCommand>
{
    private readonly IPromotionRepository _repo;

    public UpsertPromotionHandler(IPromotionRepository repo) => _repo = repo;

    public async Task Handle(UpsertPromotionCommand request, CancellationToken cancellationToken)
    {
        await _repo.UpsertAsync(new PromotionCode
        {
            Id = request.Id ?? Guid.NewGuid(),
            Code = request.Code,
            Type = request.Type,
            Value = request.Value,
            MinOrderAmount = request.MinOrderAmount,
            ValidFrom = request.ValidFrom,
            ValidTo = request.ValidTo,
            IsActive = request.IsActive,
            MaxRedemptions = request.MaxRedemptions
        }, cancellationToken);
    }
}
