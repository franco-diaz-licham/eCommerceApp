namespace Backend.Src.Api.Models.Request;

public class UpdateAddressRequest : CreateAddressRequest
{
    [Required, DeniedValues(0)] public int Id { get; set; }
}
