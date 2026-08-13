namespace LifeUniform.Web.Services;

public interface IFavoriteState
{
    IReadOnlyCollection<Guid> GetGuestIds();
    Task<IReadOnlyCollection<Guid>> GetIdsAsync(CancellationToken cancellationToken);
    Task<bool> ToggleAsync(Guid productId, CancellationToken cancellationToken);
    Task MergeGuestIntoUserAsync(string userId, CancellationToken cancellationToken);
    void ApplyGuest(IEnumerable<LifeUniform.Application.Catalog.Dto.ProductCardDto> products);
}
