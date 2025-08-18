namespace Backend.Src.Application.Interfaces;

public interface IBrandService
{
    Task<PagedList<BrandDto>> GetAllAsync(BaseQuerySpecs specs);
    Task<Result<BrandDto>> GetAsync(int id);
}