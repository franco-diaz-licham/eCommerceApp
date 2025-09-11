namespace Backend.Src.Application.Queries.Providers;

public class ProductTypeSearchProvider : ISearchEvaluatorProvider<ProductTypeEntity>
{
    public Expression<Func<ProductTypeEntity, bool>>? BuildSearch(string? searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm)) return null;
        var t = searchTerm.Trim().ToLower();
        return p => p.Name.ToLower().Contains(t);
    }
}
