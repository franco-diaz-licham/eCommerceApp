namespace Backend.Src.Api.Models;

public class PaginationMetadata
{
    public PaginationMetadata(int pageNumber, int pageSize, int totalCount, int totalPages)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;
        TotalPages = totalPages;
    }

    public int TotalCount { get; set; }
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
    public int TotalPages { get; set; }
}
