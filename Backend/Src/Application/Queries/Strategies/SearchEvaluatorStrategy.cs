namespace Backend.Src.Application.Queries.Strategies;

public sealed class SearchEvaluatorStrategy<T> : IQueryEvaluatorStrategy<T>
{
    private readonly string? _searchTerm;
    private readonly ISearchEvaluatorProvider<T> _provider;

    public SearchEvaluatorStrategy(string? searchTerm, ISearchEvaluatorProvider<T> provider)
    {
        _searchTerm = searchTerm;
        _provider = provider;
    }

    public IQueryable<T> Apply(IQueryable<T> query)
    {
        var predicate = _provider.BuildSearch(_searchTerm);
        return predicate is null ? query : query.Where(predicate);
    }
}
