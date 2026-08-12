using LifeUniform.Domain.Marketing;
using LifeUniform.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LifeUniform.Infrastructure.Marketing;

public class PromoOfferRepository : IPromoOfferRepository
{
    private readonly ApplicationDbContext _db;

    public PromoOfferRepository(ApplicationDbContext db) => _db = db;

    public async Task<IReadOnlyList<PromoOffer>> GetActiveAsync(CancellationToken cancellationToken)
    {
        return await _db.PromoOffers
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.SortOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<PromoOffer>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _db.PromoOffers
            .AsNoTracking()
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Title)
            .ToListAsync(cancellationToken);
    }

    public async Task<PromoOffer?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        await _db.PromoOffers.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task UpsertAsync(PromoOffer offer, CancellationToken cancellationToken)
    {
        var existing = await _db.PromoOffers.FirstOrDefaultAsync(x => x.Id == offer.Id, cancellationToken);
        if (existing is null)
        {
            if (offer.Id == Guid.Empty)
                offer.Id = Guid.NewGuid();
            _db.PromoOffers.Add(offer);
        }
        else
        {
            existing.Title = offer.Title;
            existing.Subtitle = offer.Subtitle;
            existing.Badge = offer.Badge;
            existing.ImageUrl = offer.ImageUrl;
            existing.LinkUrl = offer.LinkUrl;
            existing.LinkText = offer.LinkText;
            existing.Price = offer.Price;
            existing.OldPrice = offer.OldPrice;
            existing.SortOrder = offer.SortOrder;
            existing.IsActive = offer.IsActive;
        }

        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var offer = await _db.PromoOffers.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException($"Promo offer not found: {id}");
        _db.PromoOffers.Remove(offer);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
