namespace Backend.Src.Api.Models.Request;

public class CreateOrderRequest
{
    [Required] public int BasketId { get; set; }
    [Required] public CreateShippingAddressRequest ShippingAddress { get; set; } = null!;
    [Required] public CreatePaymentSummaryRequest PaymentSummary { get; set; } = null!;
}
