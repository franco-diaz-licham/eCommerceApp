namespace Backend.Src.Application.Interfaces;

public interface IBrandService
{
    IQueryable<BrandDTO> GetAllAsync(BaseQuerySpecs specs);
    Task<Result<BrandDTO>> GetAsync(int id);
}