namespace Backend.Src.Infrastructure.Services;

public class PaginationService : IPaginationService
{
    public async Task<PagedList<T>> ApplyPaginationAsync<T>(IQueryable<T> query, int pageNumber, int pageSize)
    {
        return await PagedList<T>.ToPagedList(query, pageNumber, pageSize);
    }
}
