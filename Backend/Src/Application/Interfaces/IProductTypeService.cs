namespace Backend.Src.Application.Interfaces;

public interface IProductTypeService
{
    IQueryable<ProductTypeDTO> GetAllAsync(BaseQuerySpecs specs);
    Task<ProductTypeDTO?> GetAsync(int id);
}