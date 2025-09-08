namespace Backend.Src.Api.Models.Request;

public class CreateProductRequest
{
    [Required] public required string Name { get; set; }
    [Required] public required string Description { get; set; }
    [Required] public decimal UnitPrice { get; set; }
    [Required] public int ProductTypeId { get; set; }
    [Required] public int BrandId { get; set; }
    public IFormFile? Photo { get; set; }
    [Required] public int QuantityInStock { get; set; }
}
