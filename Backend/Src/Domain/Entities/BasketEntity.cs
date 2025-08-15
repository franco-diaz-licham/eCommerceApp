namespace Backend.Src.Domain.Entities;

public class BasketEntity : BaseEntity
{
    public string? ClientSecret { get; set; }
    public string? PaymentIntentId { get; set; }
    public int? CouponId { get; set; }
    public CouponEntity? Coupon { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? UpdatedOn { get; set; }

    // Related properties
    private readonly List<BasketItemEntity> _items = new();
    public IReadOnlyCollection<BasketItemEntity> Items => _items;

    // Busines logic
    public decimal Subtotal() => _items.Sum(i => i.LineTotal());

    public void AddItem(int productId, decimal unitPrice, int quantity, ProductEntity? product = null)
    {
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));
        if (unitPrice < 0) throw new ArgumentOutOfRangeException(nameof(unitPrice));

        var existing = FindItem(productId);
        if (existing is null) _items.Add(new BasketItemEntity(productId, unitPrice, quantity, product));
        else existing.Increase(quantity);
    }

    public void RemoveItem(int productId, int quantity)
    {
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));

        var item = FindItem(productId);
        if (item is null) return;

        item.Decrease(quantity);
        if (item.Quantity == 0) _items.Remove(item);
    }

    private BasketItemEntity? FindItem(int productId) => _items.FirstOrDefault(i => i.ProductId == productId);

    public void ApplyCoupon(CouponEntity coupon)
    {
        // Validate here against domain rules (expiry, min spend, combinability, etc.)
        // Coupon = coupon;
    }
}
