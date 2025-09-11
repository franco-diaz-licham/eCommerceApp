namespace Backend.Src.Application.Queries.Interfaces;

public interface IQueryEvaluatorStrategy<T>
{
    /// <summary>
    /// Applies the query strategy to the EF core Iqueryable.
    /// </summary>
    IQueryable<T> Apply(IQueryable<T> query);
}


