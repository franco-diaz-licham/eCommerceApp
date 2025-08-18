namespace Backend.Src.Application.DTOs;

public class ProductFiltersDto
{
    public ProductFiltersDto(List<BrandDto> brands, List<ProductTypeDto> types)
    {
        Brands = brands;
        ProductTypes = types;
    }
    public List<BrandDto>? Brands { get; set; }
    public List<ProductTypeDto>? ProductTypes { get; set; }
}
