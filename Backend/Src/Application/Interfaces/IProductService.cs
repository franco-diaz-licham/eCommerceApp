namespace Backend.Src.Application.Interfaces;

public interface IProductService
{
    Task<ProductDTO> CreateAsync(ProductCreateDTO dto);
    Task<bool> DeleteAsync(int id);
    IQueryable<ProductDTO> GetAllAsync(ProductQuerySpecs specs);
    Task<ProductDTO?> GetAsync(int id);
    Task<ProductDTO> UpdateAsync(ProductUpdateDTO dto);
}