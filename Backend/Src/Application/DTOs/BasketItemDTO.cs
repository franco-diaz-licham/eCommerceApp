namespace Backend.Src.Application.DTOs;

public class BasketItemDTO
{
    public int Id { get; set; }
    public int Quantity { get; set; }
    public int ProductId { get; set; }
    public required ProductEntity Product { get; set; }
    public int BasketId { get; set; }
    public BasketDTO Basket { get; set; } = null!;
}
