namespace Backend.Src.Application.Queries.Providers;

public class OrderSearchProvider : ISearchEvaluatorProvider<OrderEntity>
{
    public Expression<Func<OrderEntity, bool>>? BuildSearch(string? searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm)) return null;
        var t = searchTerm.Trim().ToLower();
        return p => p.Id.ToString().Contains(t);
    }
}
