using LifeUniform.Domain.Marketing;
using MediatR;

namespace LifeUniform.Application.Marketing.Commands;

public class DeletePromoOfferCommand : IRequest
{
    public Guid Id { get; init; }
}

public class DeletePromoOfferHandler : IRequestHandler<DeletePromoOfferCommand>
{
    private readonly IPromoOfferRepository _repo;

    public DeletePromoOfferHandler(IPromoOfferRepository repo) => _repo = repo;

    public Task Handle(DeletePromoOfferCommand request, CancellationToken cancellationToken) =>
        _repo.DeleteAsync(request.Id, cancellationToken);
}
