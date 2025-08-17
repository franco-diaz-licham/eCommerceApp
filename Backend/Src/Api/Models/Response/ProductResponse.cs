namespace Backend.Src.Api.Models.Response;

public class ProductResponse
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public decimal UnitPrice { get; set; }
    public int ProductTypeId { get; set; }
    public required ProductTypeResponse ProductType { get; set; }
    public int BrandId { get; set; }
    public required BrandResponse Brand { get; set; }
    public int PhotoId { get; set; }
    public required PhotoResponse Photo { get; set; }
    public int QuantityInStock { get; set; }
}
