namespace LifeUniform.Application.Marketing.Dto;

public class PromoOfferDto
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
    public bool IsActive { get; set; }
}
