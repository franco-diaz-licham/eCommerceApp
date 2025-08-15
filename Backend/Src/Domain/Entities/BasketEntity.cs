namespace Backend.Src.Domain.Entities;

public class BasketEntity : BaseEntity
{
    private BasketEntity() { }
    public BasketEntity(DateTime? now = null)
    {
        CreatedOn = (now ?? DateTime.UtcNow);
    }

    #region Properties
    public string? ClientSecret { get; private set; }
    public string? PaymentIntentId { get; private set; }
    public string? CouponId { get; private set; }
    public CouponEntity? Coupon { get; private set; }
    public decimal Discount { get; private set; }
    public DateTime CreatedOn { get; private set; }
    public DateTime? UpdatedOn { get; private set; }

    // Related properties
    private readonly List<BasketItemEntity> _items = new();
    public IReadOnlyCollection<BasketItemEntity> Items => _items;

    [NotMapped] public decimal Subtotal => _items.Sum(i => i.LineTotal);
    #endregion

    #region Busines logic
    public void AddItem(int productId, decimal unitPrice, int quantity, ProductEntity? product = null)
    {
        if (quantity <= 0) throw new ArgumentOutOfRangeException("Quantity must be positive.");
        if (unitPrice < 0m) throw new ArgumentOutOfRangeException("Unit price cannot be negative.");

        var existing = FindItem(productId);
        if (existing is null) _items.Add(new BasketItemEntity(productId, unitPrice, quantity, product));
        else existing.IncreaseQuantity(quantity);
        Touch();
    }

    public void SetItemQuantity(int productId, int newQuantity)
    {
        if (newQuantity < 0) throw new ArgumentOutOfRangeException("Quantity cannot be negative.");
        var item = FindItem(productId) ?? throw new ArgumentNullException("Item not found.");
        if (newQuantity == 0) _items.Remove(item);
        else item.ReplaceQuantity(newQuantity);
        Touch();
    }

    public void RemoveItem(int productId, int quantity)
    {
        if (quantity <= 0) throw new ArgumentOutOfRangeException("Quantity must be positive.");
        var item = FindItem(productId);
        if (item is null) return;
        item.DecreaseQuantity(quantity);
        if (item.Quantity == 0) _items.Remove(item);
        Touch();
    }

    public void SetDiscount(decimal amount)
    {
        if (amount < 0m) throw new ArgumentOutOfRangeException("Discount cannot be negative.");
        Discount = Math.Min(amount, Subtotal);
        Touch();
    }

    public void AddCoupon(string couponId, CouponEntity? coupon = null)
    {
        if (string.IsNullOrWhiteSpace(couponId)) throw new ArgumentNullException("Coupon Id required.");
        CouponId = couponId;
        Coupon = coupon;
        if(coupon is not null) SetDiscount(coupon.CalculateDiscount(Subtotal));
        Touch();
    }

    public void RemoveCoupon()
    {
        CouponId = null;
        Coupon = null;
        ClearDiscount();
        Touch();
    }

    public void AttachPaymentIntent(string paymentIntentId, string clientSecret)
    {
        if (string.IsNullOrWhiteSpace(paymentIntentId)) throw new ArgumentNullException("PaymentIntentId required.");
        if (string.IsNullOrWhiteSpace(clientSecret)) throw new ArgumentNullException("ClientSecret required.");
        PaymentIntentId = paymentIntentId;
        ClientSecret = clientSecret;
        Touch();
    }

    public void ClearPaymentIntent()
    {
        PaymentIntentId = null;
        ClientSecret = null;
        Touch();
    }

    private BasketItemEntity? FindItem(int productId) => _items.FirstOrDefault(i => i.ProductId == productId);
    private void Touch() => UpdatedOn = DateTime.UtcNow;
    private void ClearDiscount() => Discount = 0m;
    #endregion
}
