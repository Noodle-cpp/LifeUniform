using FluentValidation;
using LifeUniform.Application.Abstractions.Caching;
using LifeUniform.Domain.Marketing;
using MediatR;

namespace LifeUniform.Application.Marketing.Commands;

public class UpsertClientPhotoCommand : IRequest
{
    public Guid? Id { get; init; }
    public string ImageUrl { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string? ReviewText { get; init; }
    public int Rating { get; init; } = 5;
    public int SortOrder { get; init; }
    public bool IsActive { get; init; } = true;
}

public class UpsertClientPhotoCommandValidator : AbstractValidator<UpsertClientPhotoCommand>
{
    public UpsertClientPhotoCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.ReviewText).MaximumLength(1000);
        RuleFor(x => x.ImageUrl).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Rating).InclusiveBetween(1, 5);
    }
}

public class UpsertClientPhotoHandler : IRequestHandler<UpsertClientPhotoCommand>
{
    private readonly IClientPhotoRepository _repo;
    private readonly ICatalogCacheInvalidator _cache;

    public UpsertClientPhotoHandler(IClientPhotoRepository repo, ICatalogCacheInvalidator cache)
    {
        _repo = repo;
        _cache = cache;
    }

    public async Task Handle(UpsertClientPhotoCommand request, CancellationToken cancellationToken)
    {
        await _repo.UpsertAsync(new ClientPhoto
        {
            Id = request.Id ?? Guid.NewGuid(),
            ImageUrl = request.ImageUrl,
            Title = request.Title,
            ReviewText = request.ReviewText,
            Rating = request.Rating,
            SortOrder = request.SortOrder,
            IsActive = request.IsActive
        }, cancellationToken);

        _cache.InvalidateCatalog();
    }
}
