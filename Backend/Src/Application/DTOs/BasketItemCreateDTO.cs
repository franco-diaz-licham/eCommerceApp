namespace Backend.Src.Application.DTOs;

public class BasketItemCreateDto
{
    public int? BasketId { get; set; }
    public int Quantity { get; set; }
    public int ProductId { get; set; }
}
