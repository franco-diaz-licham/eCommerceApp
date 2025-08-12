namespace Backend.Src.Application.DTOs;

public class ProductFiltersDTO
{
    public ProductFiltersDTO(List<BrandDTO> brands, List<ProductTypeDTO> types)
    {
        Brands = brands;
        ProductTypes = types;
    }
    public List<BrandDTO>? Brands { get; set; }
    public List<ProductTypeDTO>? ProductTypes { get; set; }
}
