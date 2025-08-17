namespace Backend.Src.Application.DTOs;

public class OrderItemDTO
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = default!;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? UpdatedOn { get; set; }
    public decimal LineTotal { get; set; }
}
