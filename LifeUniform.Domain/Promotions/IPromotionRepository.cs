namespace LifeUniform.Domain.Promotions;

public interface IPromotionRepository
{
    Task<PromotionCode?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<PromotionCode?> GetByCodeAsync(string code, CancellationToken cancellationToken);
    Task<IReadOnlyList<PromotionCode>> GetAllAsync(CancellationToken cancellationToken);
    Task UpsertAsync(PromotionCode promotion, CancellationToken cancellationToken);
    Task SetActiveAsync(Guid id, bool isActive, CancellationToken cancellationToken);
    Task IncrementRedemptionAsync(Guid id, CancellationToken cancellationToken);
}
