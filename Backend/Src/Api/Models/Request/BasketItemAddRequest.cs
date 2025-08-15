namespace Backend.Src.Api.Models.Request;

public class BasketItemAddRequest
{
    [Required] public int BasketId { get; set; }
    [Required] public int ProductId { get; set; }
    [Required, Range(0, int.MaxValue, ErrorMessage = "Quantity must be 0 or greater...")]  public int Quantity { get; set; }
}
