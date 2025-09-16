namespace Backend.Src.Application.DTOs;

public class OrderDto
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public required string UserId { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Total { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal Discount { get; set; }
    public required string PaymentIntentId { get; set; }
    public int OrderStatusId { get; set; }
    public OrderStatusDto? OrderStatus { get; set; }
    public ICollection<OrderItemDto> OrderItems { get; set; } = [];
    public required PaymentSummaryDto PaymentSummary { get; set; }
    public required ShippingAddressDto ShippingAddress { get; set; }

}
