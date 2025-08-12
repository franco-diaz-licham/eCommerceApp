namespace Backend.Src.Application.Queries;

public class ProductQuerySpecs : BaseQuerySpecs
{
    public List<int>? BrandIds { get; set; }
    public List<int>? ProductTypeIds { get; set; }
}
