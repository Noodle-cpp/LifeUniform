using LifeUniform.Domain.Promotions;
using MediatR;

namespace LifeUniform.Application.Promotions.Commands;

public class SetPromotionActiveCommand : IRequest
{
    public Guid Id { get; init; }
    public bool IsActive { get; init; }
}

public class SetPromotionActiveHandler : IRequestHandler<SetPromotionActiveCommand>
{
    private readonly IPromotionRepository _repo;

    public SetPromotionActiveHandler(IPromotionRepository repo) => _repo = repo;

    public Task Handle(SetPromotionActiveCommand request, CancellationToken cancellationToken) =>
        _repo.SetActiveAsync(request.Id, request.IsActive, cancellationToken);
}
