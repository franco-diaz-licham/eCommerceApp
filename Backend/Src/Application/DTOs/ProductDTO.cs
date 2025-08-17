namespace Backend.Src.Application.DTOs;

public class ProductDTO
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public decimal UnitPrice { get; set; }
    public int QuantityInStock { get; set; }
    public int ProductTypeId { get; set; }
    public ProductTypeDTO? ProductType { get; set; }
    public int BrandId { get; set; }
    public BrandDTO? Brand { get; set; }
    public int PhotoId { get; set; }
    public PhotoDTO? Photo { get; set; }
}
