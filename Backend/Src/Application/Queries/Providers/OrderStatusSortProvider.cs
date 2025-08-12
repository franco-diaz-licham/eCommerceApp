namespace Backend.Src.Application.Queries.Providers
{
    public class OrderStatusSortProvider : ISortEvaluatorProvider<OrderStatusEntity>
    {
        private readonly Dictionary<string, SortExpression<OrderStatusEntity>> _map = new(StringComparer.OrdinalIgnoreCase)
        {
            { "nameAsc", new (p => p.Name, false) },
            { "nameDesc", new (p => p.Name, true)  },
            { "createdAsc", new (p => p.CreatedOn, false) },
            { "createdDesc",new (p => p.CreatedOn, true)  }
        };

        public bool GetSorter(string? orderBy, out SortExpression<OrderStatusEntity>? sort) => _map.TryGetValue(orderBy ?? "", out sort);
    }
}
