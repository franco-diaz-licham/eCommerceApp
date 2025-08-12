namespace Backend.Src.Domain.Entities;

public class OrderEntity : BaseEntity
{
    public required string BuyerEmail { get; set; }
    public required ShippingAddress ShippingAddress { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public ICollection<ProductItemEntity> ProductItems { get; set; } = [];
    public long Subtotal { get; set; }
    public long DeliveryFee { get; set; }
    public long Discount { get; set; }
    public required string PaymentIntentId { get; set; }
    public int OrderStatusId { get; set; } = (int)OrderStatusEnum.Pending;
    public OrderStatusEntity? OrderStatus { get; set; }
    public required PaymentSummary PaymentSummary { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? UpdatedOn { get; set; }

    public long GetTotal()
    {
        return Subtotal + DeliveryFee - Discount;
    }
}
