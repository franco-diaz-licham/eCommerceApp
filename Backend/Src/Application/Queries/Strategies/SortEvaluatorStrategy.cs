namespace Backend.Src.Application.Queries.Strategies;

public sealed class SortEvaluatorStrategy<T> : IQueryEvaluatorStrategy<T>
{
    private readonly string? _orderBy;
    private readonly ISortEvaluatorProvider<T> _provider;

    public SortEvaluatorStrategy(string? orderBy, ISortEvaluatorProvider<T> provider)
    {
        _orderBy = orderBy; 
        _provider = provider;
    }

    public IQueryable<T> Apply(IQueryable<T> query)
    {
        if (_provider.GetSorter(_orderBy, out var sort) == false) return query;
        if (sort is null) return query;
        return sort.Descending ? query.OrderByDescending(sort.orderByExpression) : query.OrderBy(sort.orderByExpression);
    }
}
