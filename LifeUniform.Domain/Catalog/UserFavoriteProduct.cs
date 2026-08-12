namespace LifeUniform.Domain.Catalog;

public class UserFavoriteProduct
{
    public string UserId { get; set; } = string.Empty;

    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;
}

