namespace Backend.Src.Application.Interfaces;

/// <summary>
/// Searching provider for implementation.
/// </summary>
public interface ISearchEvaluatorProvider<T>
{
    Expression<Func<T, bool>>? BuildSearch(string? searchTerm);
}
