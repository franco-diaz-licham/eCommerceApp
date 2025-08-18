namespace Backend.Src.Application.DTOs;

public class OrderCreateDto
{
    public int BasketId { get; set; }
    public string UserEmail { get; set; } = null!;
    public AddressDto ShippingAddress { get; set; } = null!;
    public PaymentSummaryDto PaymentSummary { get; set; } = null!;
}
