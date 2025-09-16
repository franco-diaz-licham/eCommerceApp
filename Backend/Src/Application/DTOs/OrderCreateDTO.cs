namespace Backend.Src.Application.DTOs;

public class OrderCreateDto
{
    public int BasketId { get; set; }
    public ShippingAddressDto ShippingAddress { get; set; } = null!;
    public PaymentSummaryDto PaymentSummary { get; set; } = null!;
}
