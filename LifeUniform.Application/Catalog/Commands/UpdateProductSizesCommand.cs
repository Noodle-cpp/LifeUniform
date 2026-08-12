using MediatR;

namespace LifeUniform.Application.Catalog.Commands;

public class UpdateProductSizesCommand : IRequest
{
    public string Slug { get; init; } = string.Empty;
    public IReadOnlyList<Guid> SizeIds { get; init; } = Array.Empty<Guid>();
}

