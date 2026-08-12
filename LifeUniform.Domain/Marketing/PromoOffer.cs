namespace LifeUniform.Domain.Marketing;

public class PromoOffer
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Subtitle { get; set; }
    public string? Badge { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? LinkUrl { get; set; }
    public string? LinkText { get; set; }
    public decimal? Price { get; set; }
    public decimal? OldPrice { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
}

public interface IPromoOfferRepository
{
    Task<IReadOnlyList<PromoOffer>> GetActiveAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<PromoOffer>> GetAllAsync(CancellationToken cancellationToken);
    Task<PromoOffer?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task UpsertAsync(PromoOffer offer, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
