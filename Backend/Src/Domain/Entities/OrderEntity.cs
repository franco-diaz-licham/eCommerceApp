namespace Backend.Src.Domain.Entities;

public class OrderEntity : BaseEntity
{
    private OrderEntity() { }

    public OrderEntity(int userId, ShippingAddress shipping, string paymentIntentId)
    {
        SetUser(userId);
        SetShippingAddress(shipping);
        SetPaymentIntent(paymentIntentId);
        OrderDate = DateTime.UtcNow;
        OrderStatusId = (int)OrderStatusEnum.Pending;
        CreatedOn = DateTime.UtcNow;
    }

    #region Core data 
    public int UserId { get; private set; }
    public UserEntity? User { get; set; }
    public DateTime OrderDate { get; private set; } = DateTime.UtcNow;
    public decimal Subtotal { get; private set; } 
    public decimal DeliveryFee { get; private set; }
    public decimal Discount { get; private set; }
    public string PaymentIntentId { get; private set; } = default!;
    public string? LastProcessedStripeEventId { get; private set; } // idempotency guard
    public int OrderStatusId { get; private set; } = (int)OrderStatusEnum.Pending;
    public OrderStatusEntity? OrderStatus { get; private set; }
    public DateTime CreatedOn { get; private set; }
    public DateTime? UpdatedOn { get; private set; }

    // Owned aggregates
    public ShippingAddress ShippingAddress { get; private set; } = default!;
    public PaymentSummary PaymentSummary { get; private set; } = new();

    // Related
    private readonly List<OrderItemEntity> _productItems = new();
    public IReadOnlyCollection<OrderItemEntity> ProductItems => _productItems.AsReadOnly();
    [NotMapped] public decimal Total => Subtotal + DeliveryFee - Discount;
    #endregion

    #region Business Logic
    public void SetUser(int userId, UserEntity? user = null)
    {
        if (userId <= 0) throw new ArgumentException("Invalid photo id.");
        UserId = userId;
        User = user;
        Touch();
    }

    public void SetShippingAddress(ShippingAddress address)
    {
        ShippingAddress = address ?? throw new ArgumentException("Shipping address is required.");
        Touch();
    }

    public void SetPaymentIntent(string paymentIntentId)
    {
        if (string.IsNullOrWhiteSpace(paymentIntentId)) throw new ArgumentException("PaymentIntentId is required.");
        PaymentIntentId = paymentIntentId;
        Touch();
    }

    public void AddItem(int productId, string name, decimal unitPrice, int quantity)
    {
        if (quantity <= 0) throw new ArgumentException("Quantity must be positive.");
        if (unitPrice < 0m) throw new ArgumentException("Unit price cannot be negative.");

        var existing = _productItems.FirstOrDefault(i => i.ProductId == productId);
        if (existing is null) _productItems.Add(new OrderItemEntity(productId, name, unitPrice, quantity));
        else existing.IncreaseQuantity(quantity);

        RecalculateSubtotal();
        Touch();
    }

    public void RemoveItem(int productId, int quantity)
    {
        if (quantity <= 0) throw new ArgumentException("Quantity must be positive.");
        var item = _productItems.FirstOrDefault(i => i.ProductId == productId);
        if(item is null) throw new ArgumentException("Item not found.");

        item.DecreaseQuantity(quantity);
        if (item.Quantity == 0) _productItems.Remove(item);

        RecalculateSubtotal();
        Touch();
    }

    public void SetDeliveryFee(decimal fee)
    {
        if (fee < 0m) throw new ArgumentException("Delivery fee cannot be negative.");
        DeliveryFee = fee;
        Touch();
    }

    public void ApplyDiscount(decimal amount)
    {
        if (amount < 0m) throw new ArgumentException("Discount cannot be negative.");
        var maxDiscount = Subtotal + DeliveryFee;
        if (amount > maxDiscount) throw new ArgumentException("Discount exceeds order amount.");
        Discount = amount;
        Touch();
    }

    public void ClearDiscount()
    {
        Discount = 0m;
        Touch();
    }

    public void MarkPaymentSucceeded(string stripeEventId)
    {
        if (!string.IsNullOrWhiteSpace(stripeEventId) && LastProcessedStripeEventId == stripeEventId) return;
        TransitionTo(OrderStatusEnum.Paid);
        LastProcessedStripeEventId = stripeEventId;
        Touch();
    }

    public void MarkCancelled()
    {
        TransitionTo(OrderStatusEnum.Cancelled);
        Touch();
    }

    public void MarkShipped()
    {
        TransitionTo(OrderStatusEnum.Shipped);
        Touch();
    }

    public void MarkCompleted()
    {
        TransitionTo(OrderStatusEnum.Completed);
        Touch();
    }

    private static readonly Dictionary<OrderStatusEnum, OrderStatusEnum[]> AllowedTransitions = new()
    {
        { OrderStatusEnum.Pending, new[] { OrderStatusEnum.Paid, OrderStatusEnum.Cancelled, OrderStatusEnum.PaymentFailed } },
        { OrderStatusEnum.Paid, new[] { OrderStatusEnum.Shipped, OrderStatusEnum.Cancelled , OrderStatusEnum.Shipped } },
        { OrderStatusEnum.Shipped, new[] { OrderStatusEnum.Completed } },
        { OrderStatusEnum.Completed, new OrderStatusEnum[0] },
        { OrderStatusEnum.Cancelled, new OrderStatusEnum[0] },
    };

    private void TransitionTo(OrderStatusEnum next)
    {
        var current = (OrderStatusEnum)OrderStatusId;
        if (!AllowedTransitions.TryGetValue(current, out var allowed) || !allowed.Contains(next)) throw new ArgumentException($"Illegal transition {current} → {next}.");
        OrderStatusId = (int)next;
    }

    private void RecalculateSubtotal()
    {
        Subtotal = _productItems.Sum(i => i.UnitPrice * i.Quantity);
        if (Discount > Subtotal + DeliveryFee) Discount = Subtotal + DeliveryFee; 
    }

    private void Touch() => UpdatedOn = DateTime.UtcNow;
    #endregion
}
