namespace Backend.Src.Application.Queries.Providers;

public class BrandSearchProvider : ISearchEvaluatorProvider<BrandEntity>
{
    public Expression<Func<BrandEntity, bool>>? BuildSearch(string? searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm)) return null;
        var t = searchTerm.Trim().ToLower();
        return p => p.Name.ToLower().Contains(t);
    }
}
