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
    public int? CouponId { get; private set; }
    public CouponEntity? Coupon { get; private set; }
    public decimal Discount { get; private set; }
    public DateTime CreatedOn { get; private set; }
    public DateTime? UpdatedOn { get; private set; }

    // Related properties
    private readonly List<BasketItemEntity> _basketItems = [];
    public IReadOnlyCollection<BasketItemEntity> BasketItems => _basketItems;

    public decimal Subtotal => _basketItems.Sum(i => i.LineTotal);
    #endregion

    #region Busines logic
    public void AddItem(int productId, decimal unitPrice, int quantity, ProductEntity? product = null)
    {
        if (quantity <= 0) throw new ArgumentOutOfRangeException("Quantity must be positive.");
        if (unitPrice < 0m) throw new ArgumentOutOfRangeException("Unit price cannot be negative.");

        var existing = FindItem(productId);
        if (existing is null) _basketItems.Add(new BasketItemEntity(productId, unitPrice, quantity, product));
        else existing.IncreaseQuantity(quantity);
        Touch();
    }

    public void SetItemQuantity(int productId, int newQuantity)
    {
        if (newQuantity < 0) throw new ArgumentOutOfRangeException("Quantity cannot be negative.");
        var item = FindItem(productId) ?? throw new ArgumentNullException("Item not found.");
        if (newQuantity == 0) _basketItems.Remove(item);
        else item.ReplaceQuantity(newQuantity);
        Touch();
    }

    public void RemoveItem(int productId, int quantity)
    {
        if (quantity <= 0) throw new ArgumentOutOfRangeException("Quantity must be positive.");
        var item = FindItem(productId);
        if (item is null) return;
        item.DecreaseQuantity(quantity);
        if (item.Quantity == 0) _basketItems.Remove(item);
        Touch();
    }

    public void SetDiscount(decimal amount)
    {
        if (amount < 0m) throw new ArgumentOutOfRangeException("Discount cannot be negative.");
        Discount = Math.Min(amount, Subtotal);
        Touch();
    }

    public void AddCoupon(CouponEntity coupon)
    {
        if (coupon is null) throw new ArgumentNullException("Coupon Id cannot be less than zero...");
        CouponId = 0;
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

    private BasketItemEntity? FindItem(int productId) => _basketItems.FirstOrDefault(i => i.ProductId == productId);
    private void Touch() => UpdatedOn = DateTime.UtcNow;
    private void ClearDiscount() => Discount = 0m;
    #endregion
}
