namespace Backend.Src.Application.DTOs;

public class OrderCreateDTO
{
    public int BasketId { get; set; }
    public int UserId { get; set; }
    public required AddressDTO ShippingAddress { get; set; }
    public required PaymentSummaryDTO PaymentSummary { get; set; }
}
