namespace Backend.Src.Application.Interfaces;

public interface IBrandService
{
    Task<Result<List<BrandDto>>> GetAllAsync(BaseQuerySpecs specs);
    Task<Result<BrandDto>> GetAsync(int id);
}