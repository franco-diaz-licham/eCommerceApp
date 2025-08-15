namespace Backend.Src.Application.DTOs;

public class BasketItemCreateDTO
{
    public int BasketId { get; set; }
    public int Quantity { get; set; }
    public int ProductId { get; set; }
}
