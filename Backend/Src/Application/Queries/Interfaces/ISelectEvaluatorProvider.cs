namespace Backend.Src.Application.Queries.Interfaces;

public interface ISelectEvaluatorProvider<T>
{
    Expression<Func<T, bool>>? BuildSelect(string? searchTerm);
}
