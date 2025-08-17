namespace Backend.Src.Application.DTOs;

public class BasketItemDTO
{
    public int BasketId { get; private set; }
    public int ProductId { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal { get; set; }
    public ProductDTO? Product { get; set; }
}
