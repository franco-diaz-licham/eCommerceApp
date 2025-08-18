namespace Backend.Src.Application.Interfaces;

public interface IProductService
{
    Task<ProductDto> CreateAsync(ProductCreateDto Dto);
    Task<bool> DeleteAsync(int id);
    Task<PagedList<ProductDto>> GetAllAsync(ProductQuerySpecs specs);
    Task<ProductDto?> GetAsync(int id);
    Task<ProductDto> UpdateAsync(ProductUpdateDto Dto);
}