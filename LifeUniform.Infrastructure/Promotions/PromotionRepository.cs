using LifeUniform.Domain.Promotions;
using LifeUniform.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LifeUniform.Infrastructure.Promotions;

public class PromotionRepository : IPromotionRepository
{
    private readonly ApplicationDbContext _db;

    public PromotionRepository(ApplicationDbContext db) => _db = db;

    public async Task<PromotionCode?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        await _db.PromotionCodes.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<PromotionCode?> GetByCodeAsync(string code, CancellationToken cancellationToken)
    {
        var normalized = code.Trim().ToUpperInvariant();
        return await _db.PromotionCodes
            .FirstOrDefaultAsync(x => x.Code == normalized, cancellationToken);
    }

    public async Task<IReadOnlyList<PromotionCode>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _db.PromotionCodes
            .AsNoTracking()
            .OrderBy(x => x.Code)
            .ToListAsync(cancellationToken);
    }

    public async Task UpsertAsync(PromotionCode promotion, CancellationToken cancellationToken)
    {
        promotion.Code = promotion.Code.Trim().ToUpperInvariant();
        var existing = await _db.PromotionCodes.FirstOrDefaultAsync(x => x.Id == promotion.Id, cancellationToken);
        if (existing is null)
        {
            var clash = await _db.PromotionCodes.AsNoTracking()
                .AnyAsync(x => x.Code == promotion.Code, cancellationToken);
            if (clash)
                throw new InvalidOperationException($"Промокод «{promotion.Code}» уже существует.");

            if (promotion.Id == Guid.Empty)
                promotion.Id = Guid.NewGuid();
            _db.PromotionCodes.Add(promotion);
        }
        else
        {
            var clash = await _db.PromotionCodes.AsNoTracking()
                .AnyAsync(x => x.Code == promotion.Code && x.Id != promotion.Id, cancellationToken);
            if (clash)
                throw new InvalidOperationException($"Промокод «{promotion.Code}» уже существует.");

            existing.Code = promotion.Code;
            existing.Type = promotion.Type;
            existing.Value = promotion.Value;
            existing.MinOrderAmount = promotion.MinOrderAmount;
            existing.ValidFrom = promotion.ValidFrom;
            existing.ValidTo = promotion.ValidTo;
            existing.IsActive = promotion.IsActive;
            existing.MaxRedemptions = promotion.MaxRedemptions;
        }

        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task SetActiveAsync(Guid id, bool isActive, CancellationToken cancellationToken)
    {
        var promo = await _db.PromotionCodes.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException($"Promotion not found: {id}");
        promo.IsActive = isActive;
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task IncrementRedemptionAsync(Guid id, CancellationToken cancellationToken)
    {
        var promo = await _db.PromotionCodes.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException($"Promotion not found: {id}");
        promo.RedemptionCount += 1;
        await _db.SaveChangesAsync(cancellationToken);
    }
}
