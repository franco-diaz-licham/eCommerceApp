namespace Backend.Src.Application.DTOs;

public class OrderCreateDTO
{
    public int BasketId { get; set; }
    public string UserEmail { get; set; } = null!;
    public AddressDTO ShippingAddress { get; set; } = null!;
    public PaymentSummaryDTO PaymentSummary { get; set; } = null!;
}
