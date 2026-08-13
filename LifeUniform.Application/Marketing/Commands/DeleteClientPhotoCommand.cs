using LifeUniform.Application.Abstractions.Caching;
using LifeUniform.Domain.Marketing;
using MediatR;

namespace LifeUniform.Application.Marketing.Commands;

public class DeleteClientPhotoCommand : IRequest
{
    public Guid Id { get; init; }
}

public class DeleteClientPhotoHandler : IRequestHandler<DeleteClientPhotoCommand>
{
    private readonly IClientPhotoRepository _repo;
    private readonly ICatalogCacheInvalidator _cache;

    public DeleteClientPhotoHandler(IClientPhotoRepository repo, ICatalogCacheInvalidator cache)
    {
        _repo = repo;
        _cache = cache;
    }

    public async Task Handle(DeleteClientPhotoCommand request, CancellationToken cancellationToken)
    {
        await _repo.DeleteAsync(request.Id, cancellationToken);
        _cache.InvalidateCatalog();
    }
}
