namespace Backend.Src.Application.Interfaces;

public interface IProductTypeService
{
    Task<PagedList<ProductTypeDto>> GetAllAsync(BaseQuerySpecs specs);
    Task<Result<ProductTypeDto>> GetAsync(int id);
}