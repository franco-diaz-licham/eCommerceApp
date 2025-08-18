namespace Backend.Src.Api.Models.Response;

public class OrderResponse
{
    public required AddressResponse ShippingAddress { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public List<OrderItemResponse> OrderItems { get; set; } = [];
    public long Subtotal { get; set; }
    public long DeliveryFee { get; set; }
    public long Discount { get; set; }
    public required string PaymentIntentId { get; set; }
    public int OrderStatusId { get; set; }
    public OrderStatusResponse? OrderStatus { get; set; }
    public required PaymentSummaryResponse PaymentSummary { get; set; }
    public decimal Total { get; set; }
}
