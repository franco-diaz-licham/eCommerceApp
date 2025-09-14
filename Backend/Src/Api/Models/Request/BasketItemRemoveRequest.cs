namespace Backend.Src.Api.Models.Request;

public class BasketItemRemoveRequest
{
    [Required] public int BasketId { get; set; }
    [Required] public int ProductId { get; set; }
    [Required, Range(0, int.MaxValue, ErrorMessage = "Quantity must at least 1...")] public int Quantity { get; set; }
}
