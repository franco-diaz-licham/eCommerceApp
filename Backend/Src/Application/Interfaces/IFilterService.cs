namespace Backend.Src.Application.Interfaces
{
    public interface IFilterService
    {
        Task<ProductFiltersDTO> GetProductFilters();
    }
}