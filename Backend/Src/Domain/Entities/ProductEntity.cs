namespace Backend.Src.Domain.Entities;

public class ProductEntity : BaseEntity
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public decimal Price { get; set; }
    public int ProductTypeId { get; set; }
    public ProductTypeEntity? ProductType { get; set; }
    public int BrandId { get; set; }
    public BrandEntity? Brand { get; set; }
    public int PhotoId { get; set; }
    public PhotoEntity? Photo { get; set; }
    public int QuantityInStock { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? UpdatedOn { get; set; }
}
