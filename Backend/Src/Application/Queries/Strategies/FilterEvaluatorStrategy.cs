namespace Backend.Src.Application.Queries.Strategies;

public sealed class FilterEvaluatorStrategy<T, TKey> : IQueryEvaluatorStrategy<T>
{
    private readonly IEnumerable<Expression<Func<T, bool>>> _predicates;

    public FilterEvaluatorStrategy(IEnumerable<Expression<Func<T, bool>>> predicates)
    {
        _predicates = predicates ?? Enumerable.Empty<Expression<Func<T, bool>>>();
    }

    /// <summary>
    /// Applies the LINQ expression trees (e.g.Where, Contains, etc...) based on filterting for models.
    /// </summary>
    public IQueryable<T> Apply(IQueryable<T> query) => _predicates.Aggregate(query, (current, predicate) => current.Where(predicate));
}
