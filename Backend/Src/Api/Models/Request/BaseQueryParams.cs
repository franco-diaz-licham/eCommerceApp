namespace Backend.Src.Api.Models.Request;

public class BaseQueryParams
{
    public int? PageNumber { get; set; }
    public string? OrderBy { get; set; }
    public string? SearchTerm { get; set; }
}
