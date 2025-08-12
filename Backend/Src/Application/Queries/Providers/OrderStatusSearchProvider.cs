namespace Backend.Src.Application.Queries.Providers;

public class OrderStatusSearchProvider : ISearchEvaluatorProvider<OrderStatusEntity>
{
    public Expression<Func<OrderStatusEntity, bool>>? BuildSearch(string? searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm)) return null;
        var t = searchTerm.Trim().ToLower();
        return p => p.Name.ToLower().Contains(t);
    }
}
