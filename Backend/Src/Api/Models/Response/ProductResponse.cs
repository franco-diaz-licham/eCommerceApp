namespace Backend.Src.Api.Models.Response;

public class ProductResponse
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public decimal Price { get; set; }
    public int ProductTypeId { get; set; }
    public required ProductTypeEntity ProductType { get; set; }
    public int BrandId { get; set; }
    public required BrandEntity Brand { get; set; }
    public int PhotoId { get; set; }
    public required PhotoEntity Photo { get; set; }
    public int QuantityInStock { get; set; }
}
