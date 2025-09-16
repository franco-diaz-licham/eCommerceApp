namespace Backend.Src.Application.DTOs;

public class ShippingAddressDto : AddressDto
{
    public string RecipientName { get; set; } = default!;
}
