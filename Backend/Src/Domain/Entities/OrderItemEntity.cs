namespace Backend.Src.Domain.Entities;

public class OrderItemEntity
{
    public int OrderId { get; set; }
    public OrderEntity? Order { get; set; }
    public int ProductItemId { get; set; }
    public ProductItemEntity? ProductItem { get; set; }
}
