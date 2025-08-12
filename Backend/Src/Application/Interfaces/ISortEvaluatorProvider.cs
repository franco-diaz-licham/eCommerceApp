namespace Backend.Src.Application.Interfaces;

/// <summary>
/// Sorting Exression;
/// </summary>
public record SortExpression<T>(Expression<Func<T, object>> orderByExpression, bool Descending);

/// <summary>
/// Sort evaluator provider interface.
/// </summary>
public interface ISortEvaluatorProvider<T>
{
    bool GetSorter(string? orderBy, out SortExpression<T>? sort);
}
