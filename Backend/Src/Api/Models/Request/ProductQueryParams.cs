namespace Backend.Src.Api.Models.Request;

/// <summary>
/// Class which defines product based queryable parameters used for entity framework.
/// </summary>
public class ProductQueryParams : BaseQueryParams
{
    public List<int>? Brands { get; set; }
    public List<int>? ProductTypes { get; set; }
}
