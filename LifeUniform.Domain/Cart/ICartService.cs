namespace LifeUniform.Domain.Cart;

public interface ICartService
{
    IReadOnlyList<CartLine> GetItems();
    void AddOrUpdate(CartLine line);
    /// <summary>Сливает линии (например, cookie гостя после логина) в текущую корзину.</summary>
    void Merge(IEnumerable<CartLine> lines);
    void UpdateQuantity(Guid productId, Guid sizeId, int quantity, string? colorName = null);
    void Remove(Guid productId, Guid sizeId, string? colorName = null);
    void Clear();
    int GetItemCount();
    decimal GetItemsTotal();
}

