using LifeUniform.Application.Marketing.Dto;
using LifeUniform.Domain.Marketing;
using MediatR;

namespace LifeUniform.Application.Marketing.Queries;

public class GetActiveClientPhotosQuery : IRequest<IReadOnlyList<ClientPhotoDto>>;

public class GetActiveClientPhotosHandler : IRequestHandler<GetActiveClientPhotosQuery, IReadOnlyList<ClientPhotoDto>>
{
    private readonly IClientPhotoRepository _repo;

    public GetActiveClientPhotosHandler(IClientPhotoRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<ClientPhotoDto>> Handle(GetActiveClientPhotosQuery request, CancellationToken cancellationToken)
    {
        var list = await _repo.GetActiveAsync(cancellationToken);
        return list.Select(ToDto).ToList();
    }

    internal static ClientPhotoDto ToDto(ClientPhoto p) => new()
    {
        Id = p.Id,
        ImageUrl = p.ImageUrl,
        Title = p.Title,
        ReviewText = p.ReviewText,
        Rating = p.Rating,
        SortOrder = p.SortOrder,
        IsActive = p.IsActive
    };
}
