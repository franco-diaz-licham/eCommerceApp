namespace Backend.Src.Application.DTOs;

public class BasketItemDto
{
    public int BasketId { get; set; }
    public int ProductId { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal { get; set; }
    public ProductDto? Product { get; set; }
}
