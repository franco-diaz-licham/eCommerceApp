namespace Backend.Src.Api.Models.Request;

public class UpdateProductRequest
{
    [Required] public int Id { get; set; }
    [Required] public required string Name { get; set; }
    [Required] public required string Description { get; set; }
    [Required] public decimal Price { get; set; }
    [Required] public int ProductTypeId { get; set; }
    [Required] public int BrandId { get; set; }
    [Required] public PhotoCreateRequest? Photo { get; set; }
    [Required] public int QuantityInStock { get; set; }
}
