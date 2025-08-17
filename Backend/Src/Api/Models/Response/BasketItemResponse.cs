namespace Backend.Src.Api.Models.Response;

public class BasketItemResponse
{
    public int Id { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public int ProductId { get; set; }
    public decimal LineTotal { get; set; }
}
