namespace Backend.Src.Application.Queries.Strategies;

public sealed class FilterEvaluatorStrategy<T>(ProductQuerySpecs specs, IFilterExpressionProvider<T> provider) : IQueryEvaluatorStrategy<T>
{
    private readonly IFilterExpressionProvider<T> _provider = provider;
    private readonly ProductQuerySpecs _specs = specs;

    /// <summary>
    /// Applies the LINQ expression trees (e.g.Where, Contains, etc...) based on filterting for models.
    /// </summary>
    public IQueryable<T> Apply(IQueryable<T> query)
    {
        IEnumerable<Expression<Func<T, bool>>> _predicates = _provider.BuildFilter(_specs);
        return _predicates.Aggregate(query, (current, predicate) => current.Where(predicate));
    }
}
