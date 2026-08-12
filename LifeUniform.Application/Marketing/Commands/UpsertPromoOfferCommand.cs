using FluentValidation;
using LifeUniform.Domain.Marketing;
using MediatR;

namespace LifeUniform.Application.Marketing.Commands;

public class UpsertPromoOfferCommand : IRequest
{
    public Guid? Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Subtitle { get; init; }
    public string? Badge { get; init; }
    public string ImageUrl { get; init; } = string.Empty;
    public string? LinkUrl { get; init; }
    public string? LinkText { get; init; }
    public decimal? Price { get; init; }
    public decimal? OldPrice { get; init; }
    public int SortOrder { get; init; }
    public bool IsActive { get; init; } = true;
}

public class UpsertPromoOfferCommandValidator : AbstractValidator<UpsertPromoOfferCommand>
{
    public UpsertPromoOfferCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Subtitle).MaximumLength(500);
        RuleFor(x => x.Badge).MaximumLength(100);
        RuleFor(x => x.ImageUrl).NotEmpty().MaximumLength(500);
        RuleFor(x => x.LinkUrl).MaximumLength(500);
        RuleFor(x => x.LinkText).MaximumLength(100);
        RuleFor(x => x.OldPrice)
            .GreaterThan(x => x.Price)
            .When(x => x.Price is not null && x.OldPrice is not null);
    }
}

public class UpsertPromoOfferHandler : IRequestHandler<UpsertPromoOfferCommand>
{
    private readonly IPromoOfferRepository _repo;

    public UpsertPromoOfferHandler(IPromoOfferRepository repo) => _repo = repo;

    public async Task Handle(UpsertPromoOfferCommand request, CancellationToken cancellationToken)
    {
        await _repo.UpsertAsync(new PromoOffer
        {
            Id = request.Id ?? Guid.NewGuid(),
            Title = request.Title,
            Subtitle = request.Subtitle,
            Badge = request.Badge,
            ImageUrl = request.ImageUrl,
            LinkUrl = request.LinkUrl,
            LinkText = request.LinkText,
            Price = request.Price,
            OldPrice = request.OldPrice,
            SortOrder = request.SortOrder,
            IsActive = request.IsActive
        }, cancellationToken);
    }
}
