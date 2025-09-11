namespace Backend.Src.Application.Queries.Providers;

public class OrderSortProvider : ISortEvaluatorProvider<OrderEntity>
{
    private readonly Dictionary<string, SortExpression<OrderEntity>> _map = new(StringComparer.OrdinalIgnoreCase)
    {
        { "totalAsc", new (p => p.Total, false) },
        { "totalDesc", new (p => p.Total, true)  },
        { "createdAsc", new (p => p.CreatedOn, false) },
        { "createdDesc",new (p => p.CreatedOn, true)  }
    };

public bool GetSorter(string? orderBy, out SortExpression<OrderEntity>? sort) => _map.TryGetValue(orderBy ?? "", out sort);
}
