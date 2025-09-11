namespace Backend.Src.Application.Interfaces;

public interface IProductTypeService
{
    Task<Result<List<ProductTypeDto>>> GetAllAsync(BaseQuerySpecs specs);
    Task<Result<ProductTypeDto>> GetAsync(int id);
}