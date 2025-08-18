namespace Backend.Src.Api.Models.Request;

public class CreateOrderRequest
{
    [Required] public int BasketId { get; set; }
    [Required] public AddressResponse ShippingAddress { get; set; } = null!;
    [Required] public PaymentSummaryResponse PaymentSummary { get; set; } = null!;
}
