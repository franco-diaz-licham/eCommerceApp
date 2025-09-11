namespace Backend.Src.Application.Queries.Providers;

public class ProductSortProvider : ISortEvaluatorProvider<ProductEntity>
{
    private readonly Dictionary<string, SortExpression<ProductEntity>> _map = new(StringComparer.OrdinalIgnoreCase)
    {
        { "nameAsc", new (p => p.Name, false) },
        { "nameDesc", new (p => p.Name, true)  },
        { "priceAsc", new (p => p.UnitPrice, false) },
        { "priceDesc",new (p => p.UnitPrice, true)  }
    };

    public bool GetSorter(string? orderBy, out SortExpression<ProductEntity>? sort) => _map.TryGetValue(orderBy ?? "", out sort);
}
