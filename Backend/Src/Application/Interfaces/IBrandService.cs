namespace Backend.Src.Application.Interfaces;

public interface IBrandService
{
    IQueryable<BrandDTO> GetAllAsync(BaseQuerySpecs specs);
    Task<BrandDTO?> GetAsync(int id);
}