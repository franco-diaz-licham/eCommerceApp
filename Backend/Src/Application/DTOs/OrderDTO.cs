namespace Backend.Src.Application.DTOs;

public class OrderDTO
{
    public required string BuyerEmail { get; set; }
    public required AddressDTO ShippingAddress { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public ICollection<ProductItemDTO> ProductItems { get; set; } = [];
    public long Subtotal { get; set; }
    public long DeliveryFee { get; set; }
    public long Discount { get; set; }
    public required string PaymentIntentId { get; set; }
    public int OrderStatusId { get; set; }
    public OrderStatusDTO? OrderStatus { get; set; }
    public required PaymentSummaryDTO PaymentSummary { get; set; }

    public decimal Total { get; set; }
}
