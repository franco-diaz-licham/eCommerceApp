namespace Backend.Src.Application.Queries.Strategies;

public sealed class QueryStrategyContext<T>
{
    private readonly IQueryEvaluatorStrategy<T>[] _steps;
    public QueryStrategyContext(params IQueryEvaluatorStrategy<T>[] steps)
    {
        _steps = steps;
    }

    public IQueryable<T> Execute(IQueryable<T> query)
    {
        foreach (var step in _steps) query = step.Apply(query);
        return query;
    }
}
