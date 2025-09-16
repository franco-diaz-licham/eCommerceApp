namespace Backend.Src.Api.Models.Response;

public class ShippingAddressResponse : AddressResponse
{
    public string RecipientName { get; set; } = default!;
}
