namespace Backend.Src.Domain.Entities;

public class OrderEntity : BaseEntity
{
    /// <summary>
    /// Used for EF Core.
    /// </summary>
    private OrderEntity() { }

    public OrderEntity(string userEmail, ShippingAddress shipping, string paymentIntentId, decimal deliveryFee, decimal subtotal, decimal discount, PaymentSummary summary, List<OrderItemEntity> items)
    {
        SetUserEmail(userEmail);
        SetDeliveryFee(deliveryFee);
        SetSubtotal(subtotal);
        ApplyDiscount(discount);
        SetShippingAddress(shipping);
        SetPaymentIntent(paymentIntentId);
        PaymentSummary = summary;
        _orderItems = items;
        OrderDate = DateTime.UtcNow;
        OrderStatusId = (int)OrderStatusEnum.Pending;
    }

    #region Core data 
    public string UserEmail { get; private set; } = null!;
    public DateTime OrderDate { get; private set; } = DateTime.UtcNow;
    public decimal Subtotal { get; private set; } 
    public decimal DeliveryFee { get; private set; }
    public decimal Discount { get; private set; }
    public string PaymentIntentId { get; private set; } = default!;
    public string? LastProcessedStripeEventId { get; private set; }
    public int OrderStatusId { get; private set; } = (int)OrderStatusEnum.Pending;
    public OrderStatusEntity? OrderStatus { get; private set; }

    // Owned aggregates
    public ShippingAddress ShippingAddress { get; private set; } = default!;
    public PaymentSummary PaymentSummary { get; private set; } = new();

    // Related
    private readonly List<OrderItemEntity> _orderItems = [];
    public IReadOnlyCollection<OrderItemEntity> OrderItems => _orderItems.AsReadOnly();
    public decimal Total => Subtotal + DeliveryFee - Discount;
    #endregion

    #region Business Logic
    public void SetUserEmail(string userEmail)
    {
        if (string.IsNullOrWhiteSpace(userEmail)) throw new ArgumentNullException($"{nameof(userEmail)} is ivalid.");
        var email = userEmail.Trim();
        if (email.Length > 64) throw new ArgumentException($"{nameof(userEmail)} is too long.");

        UserEmail = email.ToLower();
    }

    public void SetShippingAddress(ShippingAddress address)
    {
        ShippingAddress = address ?? throw new ArgumentNullException($"{nameof(address)} is required.");
    }

    public void SetPaymentSummary(PaymentSummary summary)
    {
        PaymentSummary = summary ?? throw new ArgumentException($"{nameof(summary)} is required.");
    }

    public void SetPaymentIntent(string paymentIntentId)
    {
        if (string.IsNullOrWhiteSpace(paymentIntentId)) throw new ArgumentNullException($"{nameof(paymentIntentId)} is required.");
        PaymentIntentId = paymentIntentId;
    }

    public void AddItem(int productId, string name, decimal unitPrice, int quantity)
    {
        if (quantity <= 0) throw new ArgumentException($"{nameof(quantity)} must be positive.");
        if (unitPrice < 0m) throw new ArgumentException($"{nameof(unitPrice)} cannot be negative.");

        var existing = _orderItems.FirstOrDefault(i => i.ProductId == productId);
        if (existing is null) _orderItems.Add(new OrderItemEntity(productId, name, unitPrice, quantity));
        else existing.IncreaseQuantity(quantity);

        RecalculateSubtotal();
    }

    public void RemoveItem(int productId, int quantity)
    {
        if (quantity <= 0) throw new ArgumentException($"{nameof(quantity)} must be positive.");
        var item = _orderItems.FirstOrDefault(i => i.ProductId == productId);
        if(item is null) throw new ArgumentException("Item not found.");

        item.DecreaseQuantity(quantity);
        if (item.Quantity == 0) _orderItems.Remove(item);

        RecalculateSubtotal();
    }

    public void UpdateCharges(decimal deliveryFee, decimal subtotal, decimal discount)
    {
        SetDeliveryFee(deliveryFee);
        SetSubtotal(subtotal);
        ApplyDiscount(discount);
    }

    public void SetDeliveryFee(decimal amount)
    {
        if (amount < 0m) throw new ArgumentException($"{nameof(amount)} cannot be negative.");
        DeliveryFee = amount;
    }

    public void SetSubtotal(decimal amount)
    {
        if (amount < 0m) throw new ArgumentException($"{nameof(amount)} cannot be negative.");
        Subtotal = amount;
    }

    public void ApplyDiscount(decimal amount)
    {
        if (amount < 0m) throw new ArgumentException($"{nameof(amount)} cannot be negative.");
        var maxDiscount = Subtotal + DeliveryFee;
        if (amount > maxDiscount) throw new ArgumentException($"{nameof(amount)} exceeds order amount.");
        Discount = amount;
    }

    public void ClearDiscount()
    {
        Discount = 0m;
    }

    public void MarkPaymentSucceeded(string stripeEventId)
    {
        if (string.IsNullOrWhiteSpace(stripeEventId)) throw new ArgumentNullException($"{nameof(stripeEventId)} cannot be empty.");
        if (LastProcessedStripeEventId == stripeEventId) return;
        LastProcessedStripeEventId = stripeEventId;
        TransitionTo(OrderStatusEnum.Paid);
    }

    public void MarkCancelled()
    {
        TransitionTo(OrderStatusEnum.Cancelled);
    }

    public void MarkShipped()
    {
        TransitionTo(OrderStatusEnum.Shipped);
    }

    public void MarkCompleted()
    {
        TransitionTo(OrderStatusEnum.Completed);
    }

    public void MarkPaymentFailed()
    {
        TransitionTo(OrderStatusEnum.PaymentFailed);
    }

    /// <summary>
    /// Specifies the allowed transistions per current states.
    /// </summary>
    private static readonly Dictionary<OrderStatusEnum, OrderStatusEnum[]> AllowedTransitions = new()
    {
        { OrderStatusEnum.Pending, new[] { OrderStatusEnum.Paid, OrderStatusEnum.Cancelled, OrderStatusEnum.PaymentFailed } },
        { OrderStatusEnum.Paid, new[] { OrderStatusEnum.Shipped, OrderStatusEnum.Cancelled } },
        { OrderStatusEnum.Shipped, new[] { OrderStatusEnum.Completed } },
        { OrderStatusEnum.Completed, new OrderStatusEnum[0] },
        { OrderStatusEnum.Cancelled, new OrderStatusEnum[0] },
        { OrderStatusEnum.PaymentFailed, new OrderStatusEnum [0] }
    };

    private void TransitionTo(OrderStatusEnum next)
    {
        var current = (OrderStatusEnum)OrderStatusId;
        if (!AllowedTransitions.TryGetValue(current, out var allowed) || !allowed.Contains(next)) throw new ArgumentException($"Illegal transition.");
        OrderStatusId = (int)next;
    }

    private void RecalculateSubtotal() => Subtotal = _orderItems.Sum(i => i.UnitPrice * i.Quantity);
    #endregion
}
