namespace LifeUniform.Domain.Promotions;

public class PromotionCode
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public PromotionDiscountType Type { get; set; }
    public decimal Value { get; set; }
    public decimal? MinOrderAmount { get; set; }
    public DateTimeOffset? ValidFrom { get; set; }
    public DateTimeOffset? ValidTo { get; set; }
    public bool IsActive { get; set; } = true;
    public int? MaxRedemptions { get; set; }
    public int RedemptionCount { get; set; }
}
