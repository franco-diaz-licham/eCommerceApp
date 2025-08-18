namespace Backend.Src.Api.Models.Response;

public class AddressResponse
{
    public string Line1 { get; set; } = null!;
    public string? Line2 { get; set; } = null!;
    public string City { get; set; } = null!;
    public string State { get; set; } = null!;
    public string PostalCode { get; set; } = null!;
    public string Country { get; set; } = null!;
}
