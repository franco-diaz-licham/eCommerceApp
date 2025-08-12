namespace Backend.Src.Application.Interfaces
{
    public interface IPaginationService
    {
        Task<PagedList<T>> ApplyPaginationAsync<T>(IQueryable<T> query, int pageNumber, int pageSize);
    }
}