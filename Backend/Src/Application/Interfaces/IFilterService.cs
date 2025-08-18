namespace Backend.Src.Application.Interfaces
{
    public interface IFilterService
    {
        Task<ProductFiltersDto> GetProductFilters();
    }
}