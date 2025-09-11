namespace Backend.Src.Application.Queries.Providers;

public class ProductSearchProvider : ISearchEvaluatorProvider<ProductEntity>
{
    public Expression<Func<ProductEntity, bool>>? BuildSearch(string? searchTerm)
    {
        if (string.IsNullOrEmpty(searchTerm)) return null;
        var lowerCaseSearchTerm = searchTerm.Trim().ToLower();
        return x => x.Name.ToLower().Contains(lowerCaseSearchTerm);
    }
}
