namespace Backend.Src.Api.Models.Request;

public class BasketItemRemoveRequest
{
    [Required] public int BasketId { get; set; }
    [Required] public int ProductId { get; set; }
    [Required, AllowedValues(1, ErrorMessage = "Quantity must be 1...")] public int Quantity { get; set; }
}
