namespace Backend.Src.Application.Interfaces;

public interface IQueryEvaluatorStrategy<T>
{
    IQueryable<T> Apply(IQueryable<T> query);
}


