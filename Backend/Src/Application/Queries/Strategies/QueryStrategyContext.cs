namespace Backend.Src.Application.Queries.Strategies;

/// <summary>
/// Context class for the strategy pattern. This is the orchestrator of what strategy to be set.
/// </summary>
public sealed class QueryStrategyContext<T>(params IQueryEvaluatorStrategy<T>[] steps)
{
    private readonly IQueryEvaluatorStrategy<T>[] _steps = steps;

    public IQueryable<T> ApplyQuery(IQueryable<T> query)
    {
        foreach (var step in _steps) query = step.Apply(query);
        return query;
    }
}
