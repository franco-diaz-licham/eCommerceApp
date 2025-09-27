namespace Backend.Src.Api.Models.Request;

public class BaseQueryParams
{
    public int? PageNumber { get; set; } = 1;
    public int? PageSize { get; set; } = 12;
    public string? OrderBy { get; set; }
    public string? SearchTerm { get; set; }
}
