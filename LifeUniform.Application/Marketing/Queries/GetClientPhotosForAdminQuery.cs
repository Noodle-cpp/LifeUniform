using LifeUniform.Application.Marketing.Dto;
using LifeUniform.Domain.Marketing;
using MediatR;

namespace LifeUniform.Application.Marketing.Queries;

public class GetClientPhotosForAdminQuery : IRequest<IReadOnlyList<ClientPhotoDto>>;

public class GetClientPhotosForAdminHandler : IRequestHandler<GetClientPhotosForAdminQuery, IReadOnlyList<ClientPhotoDto>>
{
    private readonly IClientPhotoRepository _repo;

    public GetClientPhotosForAdminHandler(IClientPhotoRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<ClientPhotoDto>> Handle(GetClientPhotosForAdminQuery request, CancellationToken cancellationToken)
    {
        var list = await _repo.GetAllAsync(cancellationToken);
        return list.Select(GetActiveClientPhotosHandler.ToDto).ToList();
    }
}
