namespace Backend.Src.Api.Models;

public class PagedList<T> : List<T>
{
    public PagedList(List<T> items, int count, int pageNumber, int pageSize)
    {
        Metadata = new PaginationMetadata(pageNumber, pageSize, count, (int)Math.Ceiling(count / (double)pageSize));
        AddRange(items);
    }

    public PaginationMetadata Metadata { get; set; }

    ///// <summary>
    ///// Convert list models to a paginated wrapper.
    ///// </summary>
    //public static PagedList<T> ToPagedList(List<T> models, int count, int pageNumber, int pageSize)
    //{
    //    var items = models.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
    //    return new PagedList<T>(items, count, pageNumber, pageSize);
    //}
}
