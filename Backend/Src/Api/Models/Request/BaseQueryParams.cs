namespace Backend.Src.Api.Models.Request;

public class BaseQueryParams
{
    public int? PageNumber { get; set; }
    public int? PageSize { get; set; }
    public string? OrderBy { get; set; }
    public string? SearchTerm { get; set; }
}
