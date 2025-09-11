namespace Backend.Src.Application.Queries.Strategies;

public class SelectEvaluatorStrategy<T>(int pageNumber, int pageSize) : IQueryEvaluatorStrategy<T>
{
    public IQueryable<T> Apply(IQueryable<T> query)
    {
        return query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
    }
}
