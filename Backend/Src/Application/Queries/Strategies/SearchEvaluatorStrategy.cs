namespace Backend.Src.Application.Queries.Strategies;

public sealed class SearchEvaluatorStrategy<T>(string? searchTerm, ISearchEvaluatorProvider<T> provider) : IQueryEvaluatorStrategy<T>
{
    private readonly string? _searchTerm = searchTerm;
    private readonly ISearchEvaluatorProvider<T> _provider = provider;

    /// <summary>
    /// Applies the concrate implementation of the search provider. 
    /// </summary>
    public IQueryable<T> Apply(IQueryable<T> query)
    {
        var predicate = _provider.BuildSearch(_searchTerm);
        return predicate is null ? query : query.Where(predicate);
    }
}
