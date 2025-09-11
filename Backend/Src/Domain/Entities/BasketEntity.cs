namespace Backend.Src.Domain.Entities;

public class BasketEntity : BaseEntity
{
    public BasketEntity() { }

    public BasketEntity(string clientSecret, string paymentIntentId)
    {
        SetClientSecret(clientSecret);
        SetPaymentIntentId(paymentIntentId);
    }

    #region Properties
    public string? ClientSecret { get; private set; }
    public string? PaymentIntentId { get; private set; }
    public int? CouponId { get; private set; }
    public CouponEntity? Coupon { get; private set; }
    public decimal Discount { get; private set; }

    // Related properties
    private readonly List<BasketItemEntity> _basketItems = [];
    public IReadOnlyCollection<BasketItemEntity> BasketItems => _basketItems;
    public decimal Subtotal => _basketItems.Sum(i => i.LineTotal);
    public decimal DeliveryFee => Subtotal > 100.00m ? 0 : 5.00m;
    public decimal Total => Subtotal - Discount + DeliveryFee;
    #endregion

    #region Busines logic
    public void AddItem(int productId, decimal unitPrice, int quantity, ProductEntity? product = null)
    {
        // Validate
        if (quantity <= 0) throw new ArgumentOutOfRangeException($"{nameof(quantity)} must be positive.");
        if (unitPrice < 0m) throw new ArgumentOutOfRangeException($"{nameof(unitPrice)} cannot be negative.");

        // Add item or merge
        var existing = FindItem(productId);
        if (existing is null) _basketItems.Add(new BasketItemEntity(productId, unitPrice, quantity, product));
        else existing.IncreaseQuantity(quantity);
    }

    public void SetItemQuantity(int productId, int quantity)
    {
        // Validate
        if (quantity < 0) throw new ArgumentOutOfRangeException($"{nameof(quantity)} cannot be negative.");
        var item = FindItem(productId);
        if(item is null) throw new ArgumentNullException($"{nameof(item)} not found.");

        // Replace quantity
        if (quantity == 0) _basketItems.Remove(item);
        else item.ReplaceQuantity(quantity);
    }

    public void RemoveItem(int productId)
    {
        var item = FindItem(productId);
        if (item is null) return;
        _basketItems.Remove(item);
    }

    public void SetDiscount(decimal amount)
    {
        if (amount < 0m) throw new ArgumentOutOfRangeException($"{nameof(amount)} cannot be negative.");
        Discount = Math.Min(amount, Subtotal);
    }

    public void AddCoupon(CouponEntity coupon)
    {
        if (coupon is null) throw new ArgumentNullException($"{nameof(coupon)} is required.");
        CouponId = coupon.Id;
        Coupon = coupon;
        SetDiscount(coupon.CalculateDiscount(Subtotal));
    }

    public void RemoveCoupon()
    {
        CouponId = null;
        Coupon = null;
        ClearDiscount();
    }

    public void AttachPaymentIntent(string paymentIntentId, string clientSecret)
    {
        if (string.IsNullOrWhiteSpace(paymentIntentId)) throw new ArgumentNullException($"{nameof(paymentIntentId)} required.");
        if (string.IsNullOrWhiteSpace(clientSecret)) throw new ArgumentNullException($"{nameof(clientSecret)} required.");
        
        PaymentIntentId = paymentIntentId;
        ClientSecret = clientSecret;
    }

    public void ClearPaymentIntent()
    {
        PaymentIntentId = null;
        ClientSecret = null;
    }

    // TODO: Write unit tests for this.
    public void SetClientSecret(string clientSecret)
    {
        if (string.IsNullOrWhiteSpace(clientSecret)) throw new ArgumentNullException($"{nameof(clientSecret)} is required.");
        var trimmed = clientSecret.Trim();
        if (trimmed.Length > 256) throw new ArgumentException($"{nameof(clientSecret)} too long.");
        ClientSecret = trimmed;
    }

    // TODO: Write unit tests for this.
    public void SetPaymentIntentId(string paymentIntentId)
    {
        if (string.IsNullOrWhiteSpace(paymentIntentId)) throw new ArgumentNullException($"{nameof(paymentIntentId)} is required.");
        var trimmed = paymentIntentId.Trim();
        if (trimmed.Length > 64) throw new ArgumentException($"{nameof(paymentIntentId)} too long.");
        PaymentIntentId = trimmed;
    }

    // TODO: test this
    public long TotalToMinorUnits() => (long)Math.Round(Total * 100m, MidpointRounding.AwayFromZero);

    private BasketItemEntity? FindItem(int productId) => _basketItems.FirstOrDefault(i => i.ProductId == productId);
    private void ClearDiscount() => Discount = 0m;
    #endregion
}
