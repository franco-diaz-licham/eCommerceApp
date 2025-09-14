namespace Backend.Src.Api.Models.Response;

public class BasketItemResponse
{
    public int BasketId { get; set; }
    public int ProductId { get; set; }
    public string Name { get; set; } = default!;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public string PublicUrl { get; set; } = string.Empty;
    public decimal LineTotal { get; set; }
}
