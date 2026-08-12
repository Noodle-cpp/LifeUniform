using FluentAssertions;
using LifeUniform.Domain.Cart;

namespace LifeUniform.Tests.Unit;

public class CartMergeTests
{
    private sealed class MemoryCart : ICartService
    {
        private readonly List<CartLine> _items = new();

        public IReadOnlyList<CartLine> GetItems() => _items.AsReadOnly();

        public void AddOrUpdate(CartLine line)
        {
            var existing = _items.FirstOrDefault(x => x.ProductId == line.ProductId && x.SizeId == line.SizeId);
            if (existing is null) _items.Add(line);
            else existing.Quantity += line.Quantity;
        }

        public void Merge(IEnumerable<CartLine> lines)
        {
            foreach (var line in lines)
            {
                var existing = _items.FirstOrDefault(x => x.ProductId == line.ProductId && x.SizeId == line.SizeId);
                if (existing is null)
                    _items.Add(Clone(line));
                else
                    existing.Quantity = Math.Max(existing.Quantity, line.Quantity);
            }
        }

        public void UpdateQuantity(Guid productId, Guid sizeId, int quantity, string? colorName = null) { }
        public void Remove(Guid productId, Guid sizeId, string? colorName = null) { }
        public void Clear() => _items.Clear();
        public int GetItemCount() => _items.Sum(x => x.Quantity);
        public decimal GetItemsTotal() => _items.Sum(x => x.UnitPrice * x.Quantity);

        private static CartLine Clone(CartLine line) => new()
        {
            ProductId = line.ProductId,
            ProductName = line.ProductName,
            ProductSlug = line.ProductSlug,
            PreviewImageUrl = line.PreviewImageUrl,
            SizeId = line.SizeId,
            SizeLabel = line.SizeLabel,
            UnitPrice = line.UnitPrice,
            Quantity = line.Quantity
        };
    }

    [Fact]
    public void Merge_Combines_Quantities_By_Max()
    {
        var productId = Guid.NewGuid();
        var sizeId = Guid.NewGuid();
        var cart = new MemoryCart();
        cart.AddOrUpdate(new CartLine
        {
            ProductId = productId,
            SizeId = sizeId,
            ProductName = "A",
            ProductSlug = "a",
            SizeLabel = "M",
            UnitPrice = 100,
            Quantity = 1
        });

        cart.Merge(
        [
            new CartLine
            {
                ProductId = productId,
                SizeId = sizeId,
                ProductName = "A",
                ProductSlug = "a",
                SizeLabel = "M",
                UnitPrice = 100,
                Quantity = 3
            },
            new CartLine
            {
                ProductId = Guid.NewGuid(),
                SizeId = Guid.NewGuid(),
                ProductName = "B",
                ProductSlug = "b",
                SizeLabel = "L",
                UnitPrice = 200,
                Quantity = 1
            }
        ]);

        cart.GetItemCount().Should().Be(4);
        cart.GetItems().Should().HaveCount(2);
        cart.GetItems().First(x => x.ProductSlug == "a").Quantity.Should().Be(3);
    }
}
