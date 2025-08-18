namespace Backend.Src.Api.Models;

public class PagedList<T> : List<T>
{
    public PagedList(List<T> items, int count, int pageNumber, int pageSize)
    {
        Metadata = new PaginationMetadata(pageNumber, pageSize, count, (int)Math.Ceiling(count / (double)pageSize));
        AddRange(items);
    }

    public PaginationMetadata Metadata { get; set; }

    public static async Task<PagedList<T>> ToPagedList(IQueryable<T> query, int count, int pageNumber, int pageSize)
    {
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PagedList<T>(items, count, pageNumber, pageSize);
    }
}
