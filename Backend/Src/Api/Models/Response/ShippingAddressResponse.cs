namespace Backend.Src.Api.Models.Response;

public class ShippingAddressResponse
{
    public string RecipientName { get; set; } = default!;
    public string Line1 { get; set; } = null!;
    public string? Line2 { get; set; } = null!;
    public string City { get; set; } = null!;
    public string State { get; set; } = null!;
    public string PostalCode { get; set; } = null!;
    public string Country { get; set; } = null!;
}
