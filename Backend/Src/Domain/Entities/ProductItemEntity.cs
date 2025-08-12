namespace Backend.Src.Domain.Entities;

public class ProductItemEntity : BaseEntity
{
    public int ProductId { get; set; }
    public ProductEntity? Product { get; set; }
    public long Price { get; set; }
    public int Quantity { get; set; }
    public ICollection<OrderEntity> Orders { get; set; } = [];
}
