namespace Backend.Src.Application.Queries;

public class BaseQuerySpecs
{
    private const int MAX_PAGE_SIZE = 50;

    private const int PAGE_NUMBER = 1;

    private int _pageNumber = PAGE_NUMBER;

    public int PageNumber
    {
        get => _pageNumber;
        set => _pageNumber = value > PAGE_NUMBER ? value : PAGE_NUMBER;
    }

    private int _pageSize = 10;

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MAX_PAGE_SIZE ? MAX_PAGE_SIZE : value;
    }

    public string OrderBy { get; set; } = string.Empty;

    public string? SearchTerm { get; set; }

}
