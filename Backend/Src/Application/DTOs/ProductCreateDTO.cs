namespace Backend.Src.Application.DTOs;

public class ProductCreateDto
{
    public string Name { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public decimal UnitPrice { get; private set; }
    public int QuantityInStock { get; private set; }
    public int ProductTypeId { get; private set; }
    public int BrandId { get; private set; }
    public IFormFile? Photo { get; set; }
}
